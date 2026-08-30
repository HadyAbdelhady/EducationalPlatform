using Application.Common.Interfaces;
using Application.Features.HomeScreen.InstructorDashboard;
using Application.Features.HomeScreen.InstructorStudentEnrollments;
using Application.Features.HomeScreen.InstructorStudentExams;
using Application.Features.HomeScreen.InstructorStudentSheets;
using Application.Features.HomeScreen.InstructorStudentsProgress;
using Application.Features.HomeScreen.InstructorSchedule;
using Application.Features.HomeScreen.InstructorAttention;
using Application.Features.HomeScreen.InstructorAtRisk;
using Application.Features.HomeScreen.InstructorPayments;
using Application.Features.HomeScreen.StudentHomeScreen;
using Application.Features.HomeScreen.StudentProgress;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Features.HomeScreen
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeScreenController(
        IMediator mediator,
        ILogger<HomeScreenController> logger,
        ICurrentUserService currentUser) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<HomeScreenController> _logger = logger;
        private readonly ICurrentUserService _currentUser = currentUser;

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudentHomeScreen(Guid studentId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching home screen data for StudentId: {StudentId}", studentId);

            var query = new HomeScreenQuery { StudentId = studentId };
            var result = await _mediator.Send(query, cancellationToken);

            return result.IsSuccess
                ? Ok(result) : StatusCode((int)result.ErrorType, result);

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
            if (!_currentUser.TryGetUserId(out var studentId))
                return Unauthorized();

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
            [FromQuery] string? search = null,
            [FromQuery] Guid? educationYearId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)

        {
            if (!_currentUser.TryGetUserId(out var instructorId))
                return Unauthorized();

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
                Search = search,
                EducationYearId = educationYearId,
                Page = page,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpGet("instructor/students/{studentId}/progress")]
        public async Task<IActionResult> GetInstructorStudentEnrollments(
            Guid studentId,
            CancellationToken cancellationToken = default)
        {
            if (!_currentUser.TryGetUserId(out var instructorId))
                return Unauthorized();

            var query = new GetInstructorStudentEnrollmentsQuery
            {
                InstructorId = instructorId,
                StudentId = studentId
            };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpGet("instructor/students/{studentId}/exams")]
        public async Task<IActionResult> GetInstructorStudentExams(
            Guid studentId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            if (!_currentUser.TryGetUserId(out var instructorId))
                return Unauthorized();

            var query = new GetInstructorStudentExamsQuery
            {
                InstructorId = instructorId,
                StudentId = studentId,
                Page = page,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpGet("instructor/students/{studentId}/sheets")]
        public async Task<IActionResult> GetInstructorStudentSheets(
            Guid studentId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            if (!_currentUser.TryGetUserId(out var instructorId))
                return Unauthorized();

            var query = new GetInstructorStudentSheetsQuery
            {
                InstructorId = instructorId,
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
                : StatusCode((int)result.ErrorType, result);

        }

        [HttpGet("instructor/schedule")]
        public async Task<IActionResult> GetInstructorSchedule(
            [FromQuery] Guid educationYearId,
            [FromQuery] int days = 7,
            CancellationToken cancellationToken = default)
        {
            if (!_currentUser.TryGetUserId(out var instructorId))
                return Unauthorized();

            var query = new InstructorScheduleQuery
            {
                InstructorId = instructorId,
                EducationYearId = educationYearId,
                Days = days
            };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpGet("instructor/attention")]
        public async Task<IActionResult> GetInstructorAttention(
            [FromQuery] Guid educationYearId,
            CancellationToken cancellationToken = default)
        {
            if (!_currentUser.TryGetUserId(out var instructorId))
                return Unauthorized();

            var query = new InstructorAttentionQuery
            {
                InstructorId = instructorId,
                EducationYearId = educationYearId
            };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpGet("instructor/at-risk")]
        public async Task<IActionResult> GetInstructorAtRiskStudents(
            [FromQuery] Guid educationYearId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            if (!_currentUser.TryGetUserId(out var instructorId))
                return Unauthorized();

            var query = new InstructorAtRiskQuery
            {
                InstructorId = instructorId,
                EducationYearId = educationYearId,
                Page = page,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpGet("instructor/payments")]
        public async Task<IActionResult> GetInstructorPayments(
            [FromQuery] Guid educationYearId,
            [FromQuery] int days = 7,
            CancellationToken cancellationToken = default)
        {
            if (!_currentUser.TryGetUserId(out var instructorId))
                return Unauthorized();

            var query = new InstructorPaymentsQuery
            {
                InstructorId = instructorId,
                EducationYearId = educationYearId,
                Days = days
            };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

    }

}



