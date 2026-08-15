using Application.Common.Interfaces;
using Application.Features.Payment.CreatePaymentIntension;
using Application.Features.Payment.DTOs;
using Application.Features.Payment.DTOs.PaymobRawDtos;
using Application.Features.Payment.PaymentCallback;
using Application.Features.Payment.PaymentWebhook;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Features.Payment
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(IMediator mediator, ICurrentUserService currentUser) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ICurrentUserService _currentUser = currentUser;

        [HttpPost("Enroll")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> EnrollStudentInCourseOrSection([FromBody] PaymentInitiationRequest request, CancellationToken cancellationToken)
        {
            if (!_currentUser.TryGetUserId(out var userId))
                return Unauthorized("User id not found in token.");

            var paymentCommand = new BuyingCommand
            {
                StudentId = userId,
                EntityId = request.EntityId,
                EntityToBuy = request.EntityType,
                Money = request.Money,
                PaymentMethods = request.PaymentMethods,
            };

            var result = await _mediator.Send(paymentCommand, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpPost("Webhook")]
        public async Task<IActionResult> HandlePaymentWebhook([FromBody] PaymobWebhookPayload payload, CancellationToken cancellationToken)
        {
            try
            {
                var hmacSignature = Request.Query["hmac"].ToString();

                if (string.IsNullOrEmpty(hmacSignature))
                    hmacSignature = Request.Headers["hmac"].ToString();

                var paymentCommand = new PaymentWebhookCommand
                {
                    Payload = payload,
                    HmacSignature = hmacSignature
                };

                var result = await _mediator.Send(paymentCommand, cancellationToken);
                return Ok(result);
            }
            catch
            {
                return Ok();
            }
        }

        [HttpGet("Callback")]
        public async Task<IActionResult> PaymentCallback(CancellationToken cancellationToken)
        {
            var merchantOrderId = Request.Query["merchant_order_id"].ToString();
            var success = bool.TryParse(Request.Query["success"].ToString(), out var parsedSuccess) && parsedSuccess;

            if (!Guid.TryParse(merchantOrderId, out var paymentId))
                return BadRequest("Invalid payment reference");

            var result = await _mediator.Send(
                new PaymentCallbackCommand
                {
                    PaymentId = paymentId,
                    Success = success
                },
                cancellationToken);

            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }
    }
}
