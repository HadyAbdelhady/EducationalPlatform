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
        public async Task<IActionResult> EnrollStudentInCourseOrSection([FromBody] BuyingCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
