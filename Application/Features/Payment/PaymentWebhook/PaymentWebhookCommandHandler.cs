using Application.Features.Payment.DTOs.PaymobRawDtos;
using Application.Common.Interfaces;
using Application.Features.Auth.Interfaces;
using Application.Features.HomeScreen.Interfaces;
using Application.Features.Courses.Interfaces;
using Application.Features.Payment.Interfaces;
using Application.Common;
using Domain;
using Domain.Entities;
using Domain.enums;
using MediatR;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payment.PaymentWebhook
{
    public class PaymentWebhookCommandHandler(IPaymentService paymentService, IUnitOfWork unitOfWork) : IRequestHandler<PaymentWebhookCommand, Result<bool>>
    {
        private readonly IPaymentService _paymentService = paymentService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<bool>> Handle(PaymentWebhookCommand request, CancellationToken cancellationToken)
        {
            var payload = request.Payload.Obj;
            // ── Guard: refund webhooks must not re-enroll ────────────────────────
            if (payload.IsRefund || string.Equals(request.Payload.Type, "REFUND", StringComparison.OrdinalIgnoreCase))
            {
                return Result<bool>.Success(true);
            }

            // ── HMAC validation ──────────────────────────────────────────────────
            string concatenatedString = ConcatenatePayload(payload);

            var isValidHmac = _paymentService.VerifyHmacSignature(concatenatedString, request.HmacSignature);
            if (!isValidHmac)
            {
                return Result<bool>.FailureStatusCode("Invalid HMAC signature", ErrorType.UnAuthorized);
            }
            var isSuccess = request.Payload.Obj.Success;

            string? specialReference = payload.Order?.MerchantOrderId;

            if (string.IsNullOrEmpty(specialReference))
            {
                return Result<bool>.FailureStatusCode(
                    "Special reference (merchant_order_id) is missing from webhook payload",
                    ErrorType.BadRequest);
            }


            if (!Guid.TryParse(specialReference, out var paymentId))
            {
                return Result<bool>.FailureStatusCode(
                    $"Special reference is not a valid GUID: {specialReference}",
                    ErrorType.BadRequest);
            }
            var PaymentTransaction = await _unitOfWork.Repository<PaymentTransactions>()
                .FirstOrDefaultAsync(x => x.Id == paymentId, cancellationToken);

            if (PaymentTransaction == null)
            {
                return Result<bool>.FailureStatusCode(
                    $"Payment transaction not found: {paymentId}",
                    ErrorType.NotFound);
            }

         
            // ── Failed payment ───────────────────────────────────────────────────
            if (!isSuccess)
            {
                PaymentTransaction.Status = PaymentStatus.Failed;
                PaymentTransaction.UpdatedAt = DateTimeOffset.UtcNow;

                _unitOfWork.Repository<PaymentTransactions>().Update(PaymentTransaction);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }

            // ── Idempotency: already completed ───────────────────────────────────
            if (PaymentTransaction.Status == PaymentStatus.Completed)
            {
                return Result<bool>.Success(true);
            }


            PaymentTransaction.Status = PaymentStatus.Completed;
            PaymentTransaction.UpdatedAt = DateTimeOffset.UtcNow;

            var studentExists = await _unitOfWork.GetRepository<IUserRepository>()
                .DoesStudentExistAsync(PaymentTransaction.StudentId, cancellationToken);

            if (!studentExists)
            {
                return Result<bool>.FailureStatusCode("Student not found", ErrorType.NotFound);
            }

            // ── Resolve payee ────────────────────────────────────────────────────
            var (payeeType, payeeId) = await ResolvePayeeAsync(
                PaymentTransaction.StudentId,
                PaymentTransaction.CourseId,
                cancellationToken);

            // ── Fee math ────────────────────────────────────────────────────────
            ApplySaleFeeMath(PaymentTransaction, payload, payeeType, payeeId);

            // ── Enroll ──────────────────────────────────────────────────────────
            var enrollmentRepo = _unitOfWork.GetRepository<IStudentEnrollmentRepository>();
            await enrollmentRepo.EnrollFromPaymentAsync(
                PaymentTransaction.StudentId,
                PaymentTransaction.CourseId,
                PaymentTransaction.SectionId,
                cancellationToken);

            // ── Write ledger (idempotent) ────────────────────────────────────────
            await RecordPayoutLedgerAsync(PaymentTransaction, payeeType, payeeId, PaymentTransaction.PayeeCredit, cancellationToken);

            _unitOfWork.Repository<PaymentTransactions>().Update(PaymentTransaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            System.Diagnostics.Debug.WriteLine("=== WEBHOOK PROCESSING COMPLETED SUCCESSFULLY ===");
            return Result<bool>.Success(true);
        }

        private async Task<(PayeeType PayeeType, Guid PayeeId)> ResolvePayeeAsync(
            Guid studentId,
            Guid? courseId,
            CancellationToken cancellationToken)
        {
            var studentCenterId = await _unitOfWork.GetRepository<IUserRepository>()
                .GetStudentCenterIdAsync(studentId, cancellationToken);

            if (studentCenterId.HasValue)
                return (PayeeType.Center, studentCenterId.Value);

            if (courseId.HasValue)
            {
                var instructorId = await _unitOfWork.GetRepository<ICourseRepository>()
                    .GetCourseInstructorIdAsync(courseId.Value, cancellationToken);

                return (PayeeType.Instructor, instructorId ?? Guid.Empty);
            }

            return (PayeeType.Instructor, Guid.Empty);
        }

        private static void ApplySaleFeeMath(
            PaymentTransactions payment,
            PaymobWebhookObj payload,
            PayeeType payeeType,
            Guid payeeId)
        {
            var gross = payment.Amount;
            var paymobFeePiastres = payload.MerchantCommission ?? payload.Order?.CommissionFees ?? 0;
            var (platformFee, paymobFee, payeeCredit) = PayoutMath.ComputeSaleCredit(gross, paymobFeePiastres);

            payment.PayeeType = payeeType;
            payment.PayeeId = payeeId;
            payment.PlatformPercentFee = platformFee;
            payment.PaymobAcceptanceFee = paymobFee;
            payment.PayeeCredit = payeeCredit;
        }

        private async Task RecordPayoutLedgerAsync(
            PaymentTransactions payment,
            PayeeType payeeType,
            Guid payeeId,
            decimal payeeCredit,
            CancellationToken cancellationToken)
        {
            if (payeeId == Guid.Empty) return;

            var existingLine = await _unitOfWork.Repository<PayoutLedgerLine>()
                .FirstOrDefaultAsync(l => l.PaymentId == payment.Id, cancellationToken);

            if (existingLine != null) return;

            await _unitOfWork.Repository<PayoutLedgerLine>().AddAsync(new PayoutLedgerLine
            {
                Id = Guid.NewGuid(),
                PayeeType = payeeType,
                PayeeId = payeeId,
                Type = LedgerLineType.SaleCredit,
                Amount = payeeCredit,
                PaymentId = payment.Id,
            }, cancellationToken);

            // Upsert PayoutAccount balance
            var account = await _unitOfWork.Repository<PayoutAccount>()
                .FirstOrDefaultAsync(a => a.PayeeType == payeeType && a.PayeeId == payeeId, cancellationToken);

            if (account == null)
            {
                await _unitOfWork.Repository<PayoutAccount>().AddAsync(new PayoutAccount
                {
                    Id = Guid.NewGuid(),
                    PayeeType = payeeType,
                    PayeeId = payeeId,
                    Balance = payeeCredit,
                }, cancellationToken);
            }
            else
            {
                account.Balance += payeeCredit;
                account.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }

        private string ConcatenatePayload(PaymobWebhookObj payload)
        {
            // Paymob requires strict lexicographical concatenation of these specific fields:
            // NOTE: Must match Paymob's exact specification
            var concatenated =
                $"{payload.AmountCents}" +
                $"{payload.CreatedAt}" +
                $"{payload.Currency}" +
                $"{payload.ErrorOccured.ToString().ToLower()}" +
                $"{payload.HasParentTransaction.ToString().ToLower()}" +
                $"{payload.Id}" +
                $"{payload.IntegrationId}" +
                $"{payload.Is3DSecure.ToString().ToLower()}" +
                $"{payload.IsAuth.ToString().ToLower()}" +
                $"{payload.IsCapture.ToString().ToLower()}" +
                $"{payload.IsRefunded.ToString().ToLower()}" +
                $"{payload.IsStandalonePayment.ToString().ToLower()}" +
                $"{payload.IsVoided.ToString().ToLower()}" +
                $"{payload.Order?.Id}" +
                $"{payload.Owner}" +
                $"{payload.Pending.ToString().ToLower()}" +
                $"{payload.SourceData?.Pan}" +
                $"{payload.SourceData?.SubType}" +
                $"{payload.SourceData?.Type}" +
                $"{payload.Success.ToString().ToLower()}";

            System.Diagnostics.Debug.WriteLine($"Concatenated string for HMAC: {concatenated}");
            return concatenated;
        }
    }
}