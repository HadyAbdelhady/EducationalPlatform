using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Exams.DTOs;
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
using Application.Features.Exams.Query.GetInstructorExams;

namespace Edu_Base.Features.Exams
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController(IMediator mediator, ICurrentUserService currentUser) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ICurrentUserService _currentUser = currentUser;

        [HttpPatch("Start")]
        public async Task<IActionResult> StartExam(Guid ExamId, CancellationToken cancellationToken)
        {
            if (!_currentUser.TryGetUserId(out var userId))
                return Unauthorized("User id not found in token.");

            var command = new StartExamCommand
            {
                ExamId = ExamId,
                StudentId = userId
            };

            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpPost("Generate")]
        public async Task<IActionResult> GenerateExam([FromBody] GenerateExamCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpDelete("Delete/{examId}")]
        public async Task<IActionResult> DeleteExam(Guid examId, CancellationToken cancellationToken)
        {
            var command = new DeleteExamCommand
            {
                ExamId = examId
            };
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);

        }

        [HttpPost("Submit")]
        public async Task<IActionResult> SubmitExam([FromBody] SubmitExamRequest request, CancellationToken cancellationToken)
        {
            if (!_currentUser.TryGetUserId(out var studentId))
                return Unauthorized("User id not found in token.");

            var command = new SubmitExamCommand
            {
                ExamId = request.ExamId,
                StudentId = studentId,
                Answers = request.Answers
            };

            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpGet("GetExamById/{examId}")]
        public async Task<IActionResult> GetExamById(Guid examId, CancellationToken cancellationToken)
        {
            if (!_currentUser.TryGetUserId(out var userId))
                return Unauthorized("User id not found in token.");

            var query = new GetExamByIdQuery { Id = examId, UserId = userId };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }



        [HttpGet("GetAllExams")]
        public async Task<IActionResult> GetAllExams([FromQuery] GetAllEntityRequestSkeleton request, CancellationToken cancellationToken)
        {
            if (!_currentUser.TryGetUserId(out var userId))
                return Unauthorized("User id not found in token.");

            var query = new GetAllExamsQuery
            {
                RequestSkeleton = request,
                UserId = userId
            };

            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpGet("GetExamSubmissions")]
        public async Task<IActionResult> GetExamSubmissions([FromQuery] GetStudentsSubmittionsForExamRequest request, CancellationToken cancellationToken)
        {
            if (!_currentUser.TryGetUserId(out var instructorId))
                return Unauthorized("User id not found in token.");

            var query = new GetExamSubmissionsListQuery
            {
                RequestSkeleton = request.RequestSkeleton,
                InstructorId = instructorId,
                ExamId = request.ExamId
            };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);

        }

        [HttpGet("GetStudentExamResult/{examId}/{studentId}")]
        public async Task<IActionResult> GetStudentExamResult(Guid examId, Guid studentId, CancellationToken cancellationToken)
        {
            if (!_currentUser.TryGetUserId(out _))
                return Unauthorized("User id not found in token.");

            var query = new GetStudentExamResultQuery
            {
                ExamId = examId,
                StudentId = studentId
            };

            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpGet("GetExamCalendarDays")]
        public async Task<IActionResult> GetExamCalendarDays(
            [FromQuery] Guid? courseId,
            [FromQuery] Guid? sectionId,
            [FromQuery] Guid educationYearId,
            CancellationToken cancellationToken)
        {
            if (!_currentUser.TryGetUserId(out var instructorId))
                return Unauthorized("User id not found in token.");

            var query = new GetExamCalendarDaysQuery
            {
                CourseId = courseId,
                SectionId = sectionId,
                EducationYearId = educationYearId,
                InstructorId = instructorId
            };

            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpGet("GetInstructorExams")]
        public async Task<IActionResult> GetInstructorNonRandomExams([FromQuery] GetAllEntityRequestSkeleton request, CancellationToken cancellationToken)
        {
            if (!_currentUser.TryGetUserId(out var instructorId))
                return Unauthorized("User id not found in token.");

            var query = new GetInstructorExamsQuery
            {
                Request = new GetInstructorExamsRequest
                {
                    RequestSkeleton = request,
                    InstructorId = instructorId
                }
            };

            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }
    }
}
