using Application.Features.Exams.Command.DeleteExam;
using Application.Features.Exams.Command.GenerateExam;
using Application.Features.Exams.Command.StartExam;
using Application.Features.Exams.Command.SubmitExam;
using Application.Features.Exams.Query.Get_Exam_List;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("Start")]
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


        [HttpGet("GetExamsList")]
        public async Task<IActionResult> GetExamsList([FromQuery] GetAllExamsRequest request,CancellationToken cancellationToken)
        {
            //var userId = Guid.Parse(
            //    User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value
            //);
            var userId = Guid.Parse("d446bb09-477d-4c9e-b6fe-6971e6c80dc5");

            var query = new GetAllExamsQuery
            {
                Filters = request.Filters,
                SortBy = request.SortBy,
                IsDescending = request.IsDescending,
                PageNumber = request.PageNumber,
                UserId = userId
            };

            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : NotFound(result.Error);
        }

    }
}
