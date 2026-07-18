using Application.Features.Payment.CreatePaymentIntension;
using Application.Features.Payment.DTOs;
using Application.Features.Payment.DTOs.PaymobRawDtos;
using Application.Features.Payment.PaymentWebhook;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Features.Payment
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        [HttpPost("Enroll")]
        //[Authorize(Roles = "Student")]
        public async Task<IActionResult> EnrollStudentInCourseOrSection([FromBody] PaymentInitiationRequest request, CancellationToken cancellationToken)
        {
            //var UserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            var UserId = Guid.Parse("23669985-50b2-4e8b-9aef-9c30ea744565"); // Hardcoded for testing purposes

            var PaymentCommand = new BuyingCommand
            {
                StudentId = UserId,
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
