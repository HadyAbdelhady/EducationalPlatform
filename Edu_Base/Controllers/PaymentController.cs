using Application.DTOs.Payment;
using Application.Features.Payment.StudentBuys;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        [HttpPost("Enroll")]
        public async Task<IActionResult> EnrollStudentInCourseOrSection([FromBody] PaymentInitiationRequest request, CancellationToken cancellationToken)
        {
            var UserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

            var PaymentCommand = new BuyingCommand
            {
                StudentId = UserId,
                EntityId = request.EntityId,
                EntityToBuy = request.EntityType,
                Amount = request.Amount,
                SenderAccount = request.SenderAccount,
                ReceiverAccount = request.ReceiverAccount
            };

            var result = await _mediator.Send(PaymentCommand, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }
    }
}
