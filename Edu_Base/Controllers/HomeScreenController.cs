using Application.Features.HomeScreen.InstructorDashboard;
using Application.Features.HomeScreen.InstructorStudentsProgress;
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
                ? Ok(result) : StatusCode((int)result.ErrorType, result.Error);

        }

        [HttpGet("progress")]
        public async Task<IActionResult> GetStudentProgress(

            [FromQuery] int enrollmentsPage = 1,
            [FromQuery] int enrollmentsPageSize = 6,
            [FromQuery] int coursesPage = 0,
            [FromQuery] int coursesPageSize = 0,
            [FromQuery] int milestonesPage = 1,
            [FromQuery] int milestonesPageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var studentIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentIdClaim) || !Guid.TryParse(studentIdClaim, out var studentId))
            {
                return Unauthorized();
            }

            if (coursesPage > 0)
                enrollmentsPage = coursesPage;
            if (coursesPageSize > 0)
                enrollmentsPageSize = coursesPageSize;

            _logger.LogInformation("Fetching progress for StudentId: {StudentId}", studentId);

            var query = new StudentProgressQuery
            {
                StudentId = studentId,
                EnrollmentsPage = enrollmentsPage,
                EnrollmentsPageSize = enrollmentsPageSize,
                MilestonesPage = milestonesPage,
                MilestonesPageSize = milestonesPageSize
            };

            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);

        }

        [HttpGet("instructor/students/progress")]
        public async Task<IActionResult> GetInstructorStudentsProgress(
            [FromQuery] Guid? courseId = null,
            [FromQuery] Guid? sectionId = null,
            [FromQuery] Guid? studentId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)

        {
            var instructorIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(instructorIdClaim) || !Guid.TryParse(instructorIdClaim, out var instructorId))
            {
                return Unauthorized();
            }

            _logger.LogInformation(
                "Fetching students progress for InstructorId: {InstructorId}, CourseId: {CourseId}, SectionId: {SectionId}",
                instructorId,
                courseId,
                sectionId);

            var query = new GetInstructorStudentsProgressQuery
            {
                InstructorId = instructorId,
                CourseId = courseId,
                SectionId = sectionId,
                StudentId = studentId,
                Page = page,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpGet("instructor/{instructorId}")]
        public async Task<IActionResult> GetInstructorDashboard(Guid instructorId, [FromQuery] Guid? educationYearId = null, CancellationToken cancellationToken = default)

        {
            _logger.LogInformation("Fetching dashboard data for InstructorId: {InstructorId}", instructorId);
            var query = new InstructorDashboardQuery { InstructorId = instructorId, EducationYearId = educationYearId };
            var result = await _mediator.Send(query, cancellationToken);

            return result.IsSuccess
                ? Ok(result)
                : StatusCode((int)result.ErrorType, result.Error);

        }

    }

}


