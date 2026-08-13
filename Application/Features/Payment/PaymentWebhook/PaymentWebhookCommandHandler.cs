using Application.Features.Payment.DTOs.PaymobRawDtos;
using Application.Common.Interfaces;
using Application.Features.Auth.Interfaces;
using Application.Features.Payment.Interfaces;
using Application.Common;
using Domain.Entities;
using Domain.enums;
using MediatR;
using System.Text.Json;

namespace Application.Features.Payment.PaymentWebhook
{
    public class PaymentWebhookCommandHandler(IPaymentService paymentService, IUnitOfWork unitOfWork) : IRequestHandler<PaymentWebhookCommand, Result<bool>>
    {
        private readonly IPaymentService _paymentService = paymentService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<bool>> Handle(PaymentWebhookCommand request, CancellationToken cancellationToken)
        {
            // ✅ FIX: Add comprehensive debugging
            var payload = request.Payload.Obj;

            System.Diagnostics.Debug.WriteLine("=== WEBHOOK HANDLER STARTED ===");
            System.Diagnostics.Debug.WriteLine($"Webhook Type: {request.Payload.Type}");
            System.Diagnostics.Debug.WriteLine($"Transaction ID: {payload.Id}");
            System.Diagnostics.Debug.WriteLine($"Success: {payload.Success}");
            System.Diagnostics.Debug.WriteLine($"Order ID: {payload.Order?.Id}");
            System.Diagnostics.Debug.WriteLine($"Merchant Order ID: {payload.Order?.MerchantOrderId}");
            System.Diagnostics.Debug.WriteLine($"Amount (cents): {payload.AmountCents}");
            System.Diagnostics.Debug.WriteLine($"Currency: {payload.Currency}");
            System.Diagnostics.Debug.WriteLine($"Created At: {payload.CreatedAt}");
            System.Diagnostics.Debug.WriteLine($"HMAC Signature: {request.HmacSignature}");

            // ✅ FIX: Validate HMAC before processing
            string concatenatedString = ConcatenatePayload(payload);

            var isValidHmac = _paymentService.VerifyHmacSignature(concatenatedString, request.HmacSignature);
            if (!isValidHmac)
            {
                System.Diagnostics.Debug.WriteLine("❌ HMAC Validation FAILED - Rejecting webhook");
                return Result<bool>.FailureStatusCode("Invalid HMAC signature", ErrorType.UnAuthorized);
            }

            System.Diagnostics.Debug.WriteLine("✅ HMAC Validation PASSED");

            var isSuccess = request.Payload.Obj.Success;

            // ✅ FIX: Try to get special reference from the correct location
            // Paymob should send it as merchant_order_id in the order object
            string? specialReference = payload.Order?.MerchantOrderId;

            if (string.IsNullOrEmpty(specialReference))
            {
                System.Diagnostics.Debug.WriteLine("⚠️ MerchantOrderId is empty in Order object");
                System.Diagnostics.Debug.WriteLine($"Full Payload: {JsonSerializer.Serialize(payload)}");

                // If it's still not found, this is a problem
                return Result<bool>.FailureStatusCode(
                    "Special reference (merchant_order_id) is missing from webhook payload",
                    ErrorType.BadRequest);
            }

            System.Diagnostics.Debug.WriteLine($"Special Reference Found: {specialReference}");

            // ✅ FIX: Parse the special reference as a GUID
            if (!Guid.TryParse(specialReference, out var paymentId))
            {
                System.Diagnostics.Debug.WriteLine($"❌ Cannot parse special reference as GUID: {specialReference}");
                return Result<bool>.FailureStatusCode(
                    $"Special reference is not a valid GUID: {specialReference}",
                    ErrorType.BadRequest);
            }

            System.Diagnostics.Debug.WriteLine($"✅ Parsed Payment ID: {paymentId}");

            // ✅ FIX: Find the payment transaction in database
            var PaymentTransaction = await _unitOfWork.Repository<PaymentTransactions>()
                .FirstOrDefaultAsync(x => x.Id == paymentId, cancellationToken);

            if (PaymentTransaction == null)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Payment transaction not found in database: {paymentId}");
                return Result<bool>.FailureStatusCode(
                    $"Payment transaction not found: {paymentId}",
                    ErrorType.NotFound);
            }

            System.Diagnostics.Debug.WriteLine($"✅ Payment Transaction Found");
            System.Diagnostics.Debug.WriteLine($"   Current Status: {PaymentTransaction.Status}");
            System.Diagnostics.Debug.WriteLine($"   StudentId: {PaymentTransaction.StudentId}");
            System.Diagnostics.Debug.WriteLine($"   CourseId: {PaymentTransaction.CourseId}");
            System.Diagnostics.Debug.WriteLine($"   SectionId: {PaymentTransaction.SectionId}");
            System.Diagnostics.Debug.WriteLine($"   Amount: {PaymentTransaction.Amount}");

            // ✅ FIX: Handle failed payments
            if (!isSuccess)
            {
                System.Diagnostics.Debug.WriteLine("❌ Webhook indicates FAILED payment");
                PaymentTransaction.Status = PaymentStatus.Failed;
                PaymentTransaction.UpdatedAt = DateTimeOffset.UtcNow;

                _unitOfWork.Repository<PaymentTransactions>().Update(PaymentTransaction);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                System.Diagnostics.Debug.WriteLine("✅ Payment status updated to Failed and saved");
                return Result<bool>.Success(true);
            }

            // ✅ FIX: IDEMPOTENCY - If the payment transaction is already completed, don't process again
            if (PaymentTransaction.Status == PaymentStatus.Completed)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Payment already completed - webhook is duplicate, acknowledging and ignoring");
                // Acknowledge the webhook successfully since we already processed it
                return Result<bool>.Success(true);
            }

            System.Diagnostics.Debug.WriteLine("✅ Webhook indicates SUCCESSFUL payment - processing enrollment");

            // ✅ FIX: Update payment status
            PaymentTransaction.Status = PaymentStatus.Completed;
            PaymentTransaction.UpdatedAt = DateTimeOffset.UtcNow;

            // ✅ FIX: Get the user and student
            var User = await _unitOfWork.GetRepository<IUserRepository>()
                .GetStudentByIdWithRelationsAsync(PaymentTransaction.StudentId, cancellationToken);

            if (User == null)
            {
                System.Diagnostics.Debug.WriteLine($"❌ User not found: {PaymentTransaction.StudentId}");
                return Result<bool>.FailureStatusCode("User not found", ErrorType.NotFound);
            }

            System.Diagnostics.Debug.WriteLine($"✅ User found: {User.Id}");

            Student? student = User.Student;
            if (student == null)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Student profile not found for user: {User.Id}");
                return Result<bool>.FailureStatusCode("Student not found", ErrorType.NotFound);
            }

            System.Diagnostics.Debug.WriteLine($"✅ Student found: {student.UserId}");

            // ✅ FIX: Enroll student in course or section
            if (PaymentTransaction.CourseId.HasValue)
            {
                System.Diagnostics.Debug.WriteLine($"Enrolling in Course: {PaymentTransaction.CourseId.Value}");

                var StudentEnrollment = new StudentCourse
                {
                    StudentId = student.UserId,
                    CourseId = PaymentTransaction.CourseId.Value,
                };
                student.StudentCourses.Add(StudentEnrollment);

                System.Diagnostics.Debug.WriteLine($"✅ Course enrollment record added");
            }
            else if (PaymentTransaction.SectionId.HasValue)
            {
                System.Diagnostics.Debug.WriteLine($"Enrolling in Section: {PaymentTransaction.SectionId.Value}");

                var studentEnrollment = new StudentSection()
                {
                    StudentId = student.UserId,
                    SectionId = PaymentTransaction.SectionId.Value,
                };
                student.StudentSections.Add(studentEnrollment);

                System.Diagnostics.Debug.WriteLine($"✅ Section enrollment record added");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Payment has neither CourseId nor SectionId!");
                // This is odd, but don't fail - just mark payment as completed
            }

            // ✅ FIX: Update both PaymentTransaction and Student in the repository
            _unitOfWork.Repository<PaymentTransactions>().Update(PaymentTransaction);
            _unitOfWork.Repository<User>().Update(User);

            System.Diagnostics.Debug.WriteLine("Calling SaveChangesAsync...");
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            System.Diagnostics.Debug.WriteLine("✅ Database changes saved");

            System.Diagnostics.Debug.WriteLine("=== WEBHOOK PROCESSING COMPLETED SUCCESSFULLY ===");
            return Result<bool>.Success(true);
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