using Application.Features.HomeScreen.StudentHomeScreen;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeScreenController(IMediator mediator, ILogger<HomeScreenController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<HomeScreenController> _logger = logger;

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudentHomeScreen(Guid studentId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching home screen data for StudentId: {StudentId}", studentId);
            
            var query = new HomeScreenQuery { StudentId = studentId };
            var result = await _mediator.Send(query, cancellationToken);
            
            return result.IsSuccess 
                ? Ok(result) 
                : StatusCode((int)result.ErrorType, result.Error);
        }
    }
}

