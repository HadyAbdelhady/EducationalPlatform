using Application.Common.Interfaces;
using Application.Features.Auth.Interfaces;
using Application.Features.Payment.CreatePaymentIntension;
using Application.Features.Payment.DTOs;
using Application.Features.Payment.DTOs.PaymobRawDtos;
using Application.Features.Payment.PaymentWebhook;
using Domain.Entities;
using Domain.enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Features.Payment
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(IMediator _mediator, IUnitOfWork _unitOfWork, ICurrentUserService currentUser) : ControllerBase
    {
        [HttpPost("Enroll")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> EnrollStudentInCourseOrSection([FromBody] PaymentInitiationRequest request, CancellationToken cancellationToken)
        {
            if (!currentUser.TryGetUserId(out var userId))
                return Unauthorized("User id not found in token.");

            var PaymentCommand = new BuyingCommand
            {
                StudentId = userId,
                EntityId = request.EntityId,
                EntityToBuy = request.EntityType,
                Money = request.Money,
                PaymentMethods = request.PaymentMethods,
            };

            var result = await _mediator.Send(PaymentCommand, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpPost("Webhook")]
        public async Task<IActionResult> HandlePaymentWebhook([FromBody] PaymobWebhookPayload payload, CancellationToken cancellationToken)
        {
            try
            {
                // Try to get HMAC from query string first
                var hmacSignature = Request.Query["hmac"].ToString();

                // If not in query, try to get from request headers
                if (string.IsNullOrEmpty(hmacSignature))
                    hmacSignature = Request.Headers["hmac"].ToString();

                // If still not found, try to get from body (if payload has it)
                if (string.IsNullOrEmpty(hmacSignature) && payload != null)
                {
                    // Check if Paymob includes HMAC in the payload itself
                    // You might need to add an Hmac property to PaymobWebhookPayload
                    // hmacSignature = payload.Hmac; // ← If it exists
                }

                System.Diagnostics.Debug.WriteLine($"✅ Webhook Received");
                System.Diagnostics.Debug.WriteLine($"   HMAC from Query: {Request.Query["hmac"]}");
                System.Diagnostics.Debug.WriteLine($"   HMAC from Header: {Request.Headers["hmac"]}");
                System.Diagnostics.Debug.WriteLine($"   Final HMAC: {hmacSignature}");
                System.Diagnostics.Debug.WriteLine($"   Full Payload: {System.Text.Json.JsonSerializer.Serialize(payload)}");

                var PaymentCommand = new PaymentWebhookCommand
                {
                    Payload = payload,
                    HmacSignature = hmacSignature
                };

                var result = await _mediator.Send(PaymentCommand, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"   Stack: {ex.StackTrace}");
                return Ok(); // Return 200 anyway so Paymob doesn't retry
            }
        }

        [HttpGet("Callback")]
        public async Task<IActionResult> PaymentCallback(CancellationToken cancellationToken)
        {
            try
            {
                var queryParams = Request.Query;

                System.Diagnostics.Debug.WriteLine("=== PAYMENT CALLBACK RECEIVED ===");
                foreach (var param in queryParams)
                {
                    System.Diagnostics.Debug.WriteLine($"{param.Key}: {param.Value}");
                }

                var merchantOrderId = queryParams["merchant_order_id"].ToString();
                var success = bool.Parse(queryParams["success"].ToString() ?? "false");

                System.Diagnostics.Debug.WriteLine($"MerchantOrderId: {merchantOrderId}");
                System.Diagnostics.Debug.WriteLine($"Success: {success}");

                if (!Guid.TryParse(merchantOrderId, out var paymentId))
                {
                    System.Diagnostics.Debug.WriteLine("❌ Invalid merchant_order_id");
                    return BadRequest("Invalid payment reference");
                }

                // Get the payment from database
                var payment = await _unitOfWork.Repository<PaymentTransactions>()
                    .FirstOrDefaultAsync(x => x.Id == paymentId, cancellationToken);

                if (payment == null)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Payment not found for ID: {paymentId}");
                    return NotFound("Payment not found");
                }

                System.Diagnostics.Debug.WriteLine($"✅ Payment found: {payment.Id}");
                System.Diagnostics.Debug.WriteLine($"   Current Status: {payment.Status}");
                System.Diagnostics.Debug.WriteLine($"   StudentId: {payment.StudentId}");
                System.Diagnostics.Debug.WriteLine($"   CourseId: {payment.CourseId}");
                System.Diagnostics.Debug.WriteLine($"   SectionId: {payment.SectionId}");

                if (success)
                {
                    payment.Status = PaymentStatus.Completed;
                    payment.UpdatedAt = DateTimeOffset.UtcNow;

                    System.Diagnostics.Debug.WriteLine($"   Setting status to: {payment.Status}");

                    // Get user
                    var user = await _unitOfWork.GetRepository<IUserRepository>()
                        .GetStudentByIdWithRelationsAsync(payment.StudentId, cancellationToken);

                    System.Diagnostics.Debug.WriteLine($"   User found: {user != null}");

                    if (user?.Student != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"   Student found");

                        if (payment.CourseId.HasValue)
                        {
                            System.Diagnostics.Debug.WriteLine($"   Enrolling in course: {payment.CourseId}");
                            user.Student.StudentCourses.Add(new StudentCourse
                            {
                                StudentId = user.Id,
                                CourseId = payment.CourseId.Value,
                            });
                        }
                        else if (payment.SectionId.HasValue)
                        {
                            System.Diagnostics.Debug.WriteLine($"   Enrolling in section: {payment.SectionId}");
                            user.Student.StudentSections.Add(new StudentSection
                            {
                                StudentId = user.Id,
                                SectionId = payment.SectionId.Value,
                            });
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"   ⚠️ No CourseId or SectionId found");
                        }

                        _unitOfWork.Repository<User>().Update(user);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"   ❌ Student not found");
                    }

                    _unitOfWork.Repository<PaymentTransactions>().Update(payment);

                    System.Diagnostics.Debug.WriteLine($"   Calling SaveChangesAsync...");
                    var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
                    System.Diagnostics.Debug.WriteLine($"   SaveChanges result: {saveResult}");

                    System.Diagnostics.Debug.WriteLine("✅ Payment completed, student enrolled");
                    return Ok(new { message = "Payment processed successfully", paymentId = payment.Id });
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("❌ Payment failed, updating status");
                    payment.Status = PaymentStatus.Failed;
                    payment.UpdatedAt = DateTimeOffset.UtcNow;

                    _unitOfWork.Repository<PaymentTransactions>().Update(payment);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    return BadRequest("Payment failed");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"   StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"   Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
