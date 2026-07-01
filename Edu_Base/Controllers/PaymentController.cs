using Application.DTOs.Payment;
using Application.DTOs.Payment.PaymobRawDtos;
using Application.Features.Payment.PaymentWebhook;
using Application.Features.Payment.StudentBuys;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        [HttpPost("Enroll")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> EnrollStudentInCourseOrSection([FromBody] PaymentInitiationRequest request, CancellationToken cancellationToken)
        {
            var UserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            var UserName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var UserEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            //var UserId = Guid.Parse("1c231d0f-006e-47ac-a589-59f42f63c94c"); // Hardcoded for testing purposes

            var PaymentCommand = new BuyingCommand
            {
                StudentId = UserId,
                Student = new Student(UserName?.Split(" ")?[0], UserName?.Split(" ")?[1], UserEmail),
                EntityId = request.EntityId,
                EntityToBuy = request.EntityType,
                Money = request.Money
            };

            var result = await _mediator.Send(PaymentCommand, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpPost("Webhook")]
        public async Task<IActionResult> HandlePaymentWebhook([FromBody] PaymobWebhookPayload payload, CancellationToken cancellationToken)
        {
            // Extract the HMAC signature from the headers (Paymob usually sends it in "hmac")
            var hmacSignature = Request.Query["hmac"].ToString();

            var PaymentCommand = new PaymentWebhookCommand
            {
                Payload = payload,
                HmacSignature = hmacSignature
            };
            
            var result = await _mediator.Send(PaymentCommand, cancellationToken);
            
            // Webhooks MUST ALWAYS return 200 OK to the provider, even if your internal logic fails.
            // Otherwise, Paymob will assume you didn't receive it and will keep spamming the endpoint.
            return Ok(result);
        }
    }
}
