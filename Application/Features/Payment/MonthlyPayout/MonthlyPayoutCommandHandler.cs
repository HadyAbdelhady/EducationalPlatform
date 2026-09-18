using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Payment.DTOs;
using Application.Features.Payment.Interfaces;
using Domain;
using Domain.Entities;
using Domain.enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Features.Payment.MonthlyPayout
{
    public class MonthlyPayoutCommandHandler(
        IUnitOfWork unitOfWork,
        IPaymentService paymentService,
        IPayoutRepository payoutRepository,
        IOptions<PaymobSettings> settings,
        ILogger<MonthlyPayoutCommandHandler> logger) : IRequestHandler<MonthlyPayoutCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IPaymentService _paymentService = paymentService;
        private readonly IPayoutRepository _payoutRepository = payoutRepository;
        private readonly PaymobSettings _settings = settings.Value;
        private readonly ILogger<MonthlyPayoutCommandHandler> _logger = logger;

        private static readonly TimeZoneInfo Cairo = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");

        public async Task<Result<bool>> Handle(MonthlyPayoutCommand request, CancellationToken cancellationToken)
        {
            var cairoNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Cairo);
            var period = cairoNow.AddMonths(-1);
            var periodYear = period.Year;
            var periodMonth = period.Month;

            _logger.LogInformation("MonthlyPayout starting for period {Year}-{Month}", periodYear, periodMonth);

            // ── Process all non-deleted Centers ──────────────────────────────────
            var centers = await _payoutRepository.GetAllActiveCentersAsync(cancellationToken);
            foreach (var center in centers)
            {
                await ProcessPayeeAsync(
                    PayeeType.Center, center.Id, center.PaymobRecipientId,
                    periodYear, periodMonth, cancellationToken);
            }

            // ── Process independent Instructors (no CenterInstructor rows) ────────
            var independentInstructors = await _payoutRepository.GetAllIndependentInstructorsAsync(cancellationToken);
            foreach (var instructor in independentInstructors)
            {
                await ProcessPayeeAsync(
                    PayeeType.Instructor, instructor.UserId, instructor.PaymobRecipientId,
                    periodYear, periodMonth, cancellationToken);
            }

            _logger.LogInformation("MonthlyPayout completed for period {Year}-{Month}", periodYear, periodMonth);
            return Result<bool>.Success(true);
        }

        private async Task ProcessPayeeAsync(
            PayeeType payeeType, Guid payeeId, string? paymobRecipientId,
            int periodYear, int periodMonth, CancellationToken cancellationToken)
        {
            // ── Idempotency: skip if a non-Pending batch already exists ──────────
            var existingBatch = await _unitOfWork.Repository<PayoutBatch>()
                .FirstOrDefaultAsync(
                    b => b.PayeeType == payeeType && b.PayeeId == payeeId &&
                         b.PeriodYear == periodYear && b.PeriodMonth == periodMonth,
                    cancellationToken);

            if (existingBatch != null && existingBatch.Status != PayoutBatchStatus.Pending)
            {
                _logger.LogInformation("Skipping {PayeeType} {PayeeId} — batch already {Status}", payeeType, payeeId, existingBatch.Status);
                return;
            }

            // ── Load unclaimed ledger lines for this period ───────────────────────
            var unclaimedLines = await _unitOfWork.Repository<PayoutLedgerLine>()
                .Find(l => l.PayeeType == payeeType && l.PayeeId == payeeId &&
                           l.PayoutBatchId == null &&
                           l.CreatedAt.Year == periodYear && l.CreatedAt.Month == periodMonth,
                    cancellationToken)
                .ToListAsync(cancellationToken);

            var credits = unclaimedLines
                .Where(l => l.Type == LedgerLineType.SaleCredit)
                .Sum(l => l.Amount);

            var refundReversals = unclaimedLines
                .Where(l => l.Type == LedgerLineType.RefundReversal)
                .Sum(l => l.Amount); // already negative

            // ── Get or create PayoutAccount ──────────────────────────────────────
            var account = await _unitOfWork.Repository<PayoutAccount>()
                .FirstOrDefaultAsync(a => a.PayeeType == payeeType && a.PayeeId == payeeId, cancellationToken);

            decimal openingBalance = account?.Balance ?? 0m;

            var net = PayoutMath.ComputeMonthlyNet(openingBalance, credits, refundReversals, _settings.MonthlyFee);

            // ── Create or reuse batch ─────────────────────────────────────────────
            var batch = existingBatch ?? new PayoutBatch
            {
                Id = Guid.NewGuid(),
                PayeeType = payeeType,
                PayeeId = payeeId,
                PeriodYear = periodYear,
                PeriodMonth = periodMonth,
            };

            batch.Credits = credits;
            batch.SubscriptionFee = _settings.MonthlyFee;
            batch.Net = net;
            batch.UpdatedAt = DateTimeOffset.UtcNow;

            if (existingBatch == null)
                await _unitOfWork.Repository<PayoutBatch>().AddAsync(batch, cancellationToken);
            else
                _unitOfWork.Repository<PayoutBatch>().Update(batch);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // ── Attach unclaimed lines ────────────────────────────────────────────
            foreach (var line in unclaimedLines)
            {
                line.PayoutBatchId = batch.Id;
                _unitOfWork.Repository<PayoutLedgerLine>().Update(line);
            }

            // ── Monthly subscription line ─────────────────────────────────────────
            await _unitOfWork.Repository<PayoutLedgerLine>().AddAsync(new PayoutLedgerLine
            {
                Id = Guid.NewGuid(),
                PayeeType = payeeType,
                PayeeId = payeeId,
                Type = LedgerLineType.MonthlySubscription,
                Amount = -_settings.MonthlyFee,
                PayoutBatchId = batch.Id,
            }, cancellationToken);

            // ── Attempt Paymob Send ───────────────────────────────────────────────
            if (net > 0 && !string.IsNullOrWhiteSpace(paymobRecipientId))
            {
                var sendId = await _paymentService.SendPayoutAsync(
                    paymobRecipientId, net,
                    $"Payout {periodYear}-{periodMonth:D2} {payeeType} {payeeId}",
                    cancellationToken);

                batch.Status = sendId != null ? PayoutBatchStatus.Sent : PayoutBatchStatus.Failed;
                if (sendId != null) batch.PaymobSendId = sendId;

                UpdateAccountBalance(account, payeeType, payeeId, sendId != null ? 0m : net);
            }
            else if (net > 0)
            {
                batch.Status = PayoutBatchStatus.SkippedNoRecipient;
                UpdateAccountBalance(account, payeeType, payeeId, net);
            }
            else
            {
                batch.Status = PayoutBatchStatus.Pending;
                UpdateAccountBalance(account, payeeType, payeeId, net);
            }

            if (account == null)
            {
                await _unitOfWork.Repository<PayoutAccount>().AddAsync(new PayoutAccount
                {
                    Id = Guid.NewGuid(),
                    PayeeType = payeeType,
                    PayeeId = payeeId,
                    Balance = net,
                }, cancellationToken);
            }

            _unitOfWork.Repository<PayoutBatch>().Update(batch);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAccountBalance(PayoutAccount? account, PayeeType payeeType, Guid payeeId, decimal newBalance)
        {
            if (account == null) return;
            account.Balance = newBalance;
            account.UpdatedAt = DateTimeOffset.UtcNow;
            _unitOfWork.Repository<PayoutAccount>().Update(account);
        }
    }
}
