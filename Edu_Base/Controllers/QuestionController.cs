using Application.Features.Question.Command.AddQuestion;
using Application.Features.Question.Command.DeleteQuestion;
using Application.Features.Question.Command.UpdateQuestion;
using Application.Features.Question.Query.GetQuestionById;
using Application.Features.Question.Query.GetAllQuestionsInBank;
using Application.Features.Question.Query.GetAllQuestionsInExam;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController(IMediator mediator, ILogger<QuestionController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<QuestionController> _logger = logger;

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] AddQuestionCommand command)
        {

            _logger.LogInformation("Fetching course detail for CourseId: {CourseId}", command.QuestionString);
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuestionCommand command)
        {
            if (id != command.QuestionId)
            {
                return BadRequest();
            }

            var result = await _mediator.Send(command);
            return result.IsSuccess ? NoContent() : NotFound(result.Error);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteQuestionCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? NoContent() : NotFound(result.Error);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestionById(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching question detail for QuestionId: {QuestionId}", id);
            var query = new GetQuestionByIdQuery { QuestionId = id };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        [HttpGet("bank/{bankId}")]
        public async Task<IActionResult> GetAllQuestionsInBank(Guid bankId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all questions in bank: {BankId}", bankId);
            var query = new GetAllQuestionsInBankQuery { BankId = bankId };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        [HttpGet("exam/{examId}")]
        public async Task<IActionResult> GetAllQuestionsInExam(Guid examId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all questions in exam: {ExamId}", examId);
            var query = new GetAllQuestionsInExamQuery { ExamId = examId };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }
    }
}