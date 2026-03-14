using Application.Features.HomeScreen.InstructorDashboard;
using Application.Features.HomeScreen.StudentHomeScreen;
using Application.Features.HomeScreen.StudentProgress;
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

        [HttpGet("progress")]
        public async Task<IActionResult> GetStudentProgress(
            [FromQuery] int coursesPage = 1,
            [FromQuery] int coursesPageSize = 6,
            [FromQuery] int milestonesPage = 1,
            [FromQuery] int milestonesPageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var studentIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentIdClaim) || !Guid.TryParse(studentIdClaim, out var studentId))
            {
                return Unauthorized();
            }

            _logger.LogInformation("Fetching progress for StudentId: {StudentId}", studentId);

            var query = new StudentProgressQuery
            {
                StudentId = studentId,
                CoursesPage = coursesPage,
                CoursesPageSize = coursesPageSize,
                MilestonesPage = milestonesPage,
                MilestonesPageSize = milestonesPageSize
            };
            var result = await _mediator.Send(query, cancellationToken);

            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpGet("instructor/{instructorId}")]
        public async Task<IActionResult> GetInstructorDashboard(Guid instructorId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching dashboard data for InstructorId: {InstructorId}", instructorId);
            
            var query = new InstructorDashboardQuery { InstructorId = instructorId };
            var result = await _mediator.Send(query, cancellationToken);
            
            return result.IsSuccess 
                ? Ok(result) 
                : StatusCode((int)result.ErrorType, result.Error);
        }
    }
}

