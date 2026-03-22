using Application.DTOs;
using Application.DTOs.Exam;
using Application.Features.Exams.Command.DeleteExam;
using Application.Features.Exams.Command.GenerateExam;
using Application.Features.Exams.Command.StartExam;
using Application.Features.Exams.Command.SubmitExam;
using Application.Features.Exams.Query.GetAllStudentExams;
using Application.Features.Exams.Query.GetExamCalendarDays;
using Application.Features.Exams.Query.GetExamById;
using Application.Features.Exams.Query.GetExamSubmissionsList;
using Application.Features.Exams.Query.GetStudentExamResult;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Edu_Base.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPatch("Start")]
        public async Task<IActionResult> StartExam([FromBody] StartExamCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
        }

        [HttpPost("Generate")]
        public async Task<IActionResult> GenerateExam([FromBody] GenerateExamCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
        }

        [HttpDelete("Delete/{examId}")]
        public async Task<IActionResult> DeleteExam(Guid examId, CancellationToken cancellationToken)
        {
            var command = new DeleteExamCommand
            {
                ExamId = examId
            };
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);

        }

        [HttpPost("Submit")]
        public async Task<IActionResult> SubmitExam([FromBody] SubmitExamCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpGet("GetExamById/{examId}")]
        public async Task<IActionResult> GetExamById(Guid examId, CancellationToken cancellationToken)
        {

            var UserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            //Guid UserId = Guid.Parse("d446bb09-477d-4c9e-b6fe-6971e6c80dc5");
            var query = new GetExamByIdQuery { Id = examId, UserId = UserId };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }



        [HttpGet("GetStudentExams")]
        public async Task<IActionResult> GetStudentExams([FromQuery] GetAllEntityRequestSkeleton request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );
            //var userId = Guid.Parse("d446bb09-477d-4c9e-b6fe-6971e6c80dc5");

            var query = new GetAllStudentExamsQuery
            {
                RequestSkeleton = request,
                UserId = userId
            };

            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : NotFound(result.Error);
        }

        [HttpGet("GetExamSubmissions")]
        public async Task<IActionResult> GetExamSubmissions([FromQuery] GetStudentsSubmittionsForExamRequest request, CancellationToken cancellationToken)
        {
            var Instructor = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );
            var query = new GetExamSubmissionsListQuery
            {
                RequestSkeleton = request.RequestSkeleton,
                UserId = Instructor,
                ExamId = request.ExamId
            };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : NotFound(result.Error);

        }

        [HttpGet("GetStudentExamResult/{examId}")]
        public async Task<IActionResult> GetStudentExamResult(Guid examId, CancellationToken cancellationToken)
        {
            var studentIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var studentIdValue = "d446bb09-477d-4c9e-b6fe-6971e6c80dc5";
            if (string.IsNullOrWhiteSpace(studentIdValue) || !Guid.TryParse(studentIdValue, out var studentId))
            {
                return Unauthorized("User id not found in token.");
            }

            var query = new GetStudentExamResultQuery
            {
                ExamId = examId,
                StudentId = studentId
            };

            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : NotFound(result.Error);
        }

        [HttpGet("GetExamCalendarDays")]
        public async Task<IActionResult> GetExamCalendarDays(
            [FromQuery] Guid? courseId,
            [FromQuery] Guid? sectionId,
            [FromQuery] Guid educationYearId,
            CancellationToken cancellationToken)
        {
            var instructorIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var instructorIdValue = "1eb4f9e5-0327-47d5-a18c-ea1119d57827";

            if (string.IsNullOrWhiteSpace(instructorIdValue) || !Guid.TryParse(instructorIdValue, out var instructorId))
            {
                return Unauthorized("User id not found in token.");
            }

            var query = new GetExamCalendarDaysQuery
            {
                CourseId = courseId,
                SectionId = sectionId,
                EducationYearId = educationYearId,
                InstructorId = instructorId
            };

            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
        }
    }
}
