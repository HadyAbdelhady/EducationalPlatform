using Application.Common;
using Application.Common.Interfaces;
using Application.Features.HomeScreen.Interfaces;
using Application.Features.Payment.DTOs;
using Application.Features.Payment.Interfaces;
using Domain;
using Domain.Entities;
using Domain.enums;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.Features.Payment.Refund
{
    public class RefundCommandHandler(
        IPaymentService paymentService,
        IUnitOfWork unitOfWork,
        IOptions<PaymobSettings> settings) : IRequestHandler<RefundCommand, Result<bool>>
    {
        private readonly IPaymentService _paymentService = paymentService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly PaymobSettings _settings = settings.Value;

        public async Task<Result<bool>> Handle(RefundCommand request, CancellationToken cancellationToken)
        {
            var payment = await _unitOfWork.Repository<PaymentTransactions>()
                .FirstOrDefaultAsync(p => p.Id == request.PaymentId, cancellationToken);

            if (payment == null)
                return Result<bool>.FailureStatusCode("Payment not found.", ErrorType.NotFound);

            if (payment.StudentId != request.StudentId)
                return Result<bool>.FailureStatusCode("Payment does not belong to this student.", ErrorType.Forbidden);

            if (payment.Status != PaymentStatus.Completed)
                return Result<bool>.FailureStatusCode("Only completed payments can be refunded.", ErrorType.BadRequest);

            var refundDeadline = payment.CreatedAt.AddDays(_settings.RefundWindowDays);
            if (DateTimeOffset.UtcNow > refundDeadline)
                return Result<bool>.FailureStatusCode(
                    $"Refund window of {_settings.RefundWindowDays} days has expired.", ErrorType.BadRequest);

            // ── Call Paymob refund API ────────────────────────────────────────
            var refundPiastres = PayoutMath.ComputeRefundPiastres(payment.Amount);
            var refundTxnId = await _paymentService.RefundAsync(
                payment.PaymobIntentionId ?? payment.Id.ToString(),
                refundPiastres,
                cancellationToken);

            if (refundTxnId == null)
                return Result<bool>.FailureStatusCode("Paymob refund request failed.", ErrorType.InternalServerError);

            // ── Mark payment refunded ─────────────────────────────────────────
            payment.Status = PaymentStatus.Refunded;
            payment.UpdatedAt = DateTimeOffset.UtcNow;
            _unitOfWork.Repository<PaymentTransactions>().Update(payment);

            // ── Reverse payee credit (90% of gross, not platform cut) ─────────
            if (payment.PayeeId.HasValue && payment.PayeeId != Guid.Empty && payment.PayeeType.HasValue)
            {
                var reversal = PayoutMath.ComputeRefundReversal(payment.Amount);

                await _unitOfWork.Repository<PayoutLedgerLine>().AddAsync(new PayoutLedgerLine
                {
                    Id = Guid.NewGuid(),
                    PayeeType = payment.PayeeType.Value,
                    PayeeId = payment.PayeeId.Value,
                    Type = LedgerLineType.RefundReversal,
                    Amount = -reversal,
                    PaymentId = payment.Id,
                }, cancellationToken);

                var account = await _unitOfWork.Repository<PayoutAccount>()
                    .FirstOrDefaultAsync(
                        a => a.PayeeType == payment.PayeeType.Value && a.PayeeId == payment.PayeeId.Value,
                        cancellationToken);

                if (account != null)
                {
                    account.Balance -= reversal;
                    account.UpdatedAt = DateTimeOffset.UtcNow;
                    _unitOfWork.Repository<PayoutAccount>().Update(account);
                }
            }

            // ── Unenroll student ──────────────────────────────────────────────
            var enrollmentRepo = _unitOfWork.GetRepository<IStudentEnrollmentRepository>();
            await enrollmentRepo.UnenrollFromPaymentAsync(
                payment.StudentId,
                payment.CourseId,
                payment.SectionId,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
