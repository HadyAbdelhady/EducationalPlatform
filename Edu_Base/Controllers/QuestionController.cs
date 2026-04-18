using Application.DTOs.Questions;
using Application.Features.Answers.Command.AnswerQuestion;
using Application.Features.Questions.Command.AddQuestion;
using Application.Features.Questions.Command.DeleteQuestion;
using Application.Features.Questions.Command.UpdateQuestion;
using Application.Features.Questions.Query.GetAllQuestionsInExam;
using Application.Features.Questions.Query.GetAllQuestionsWithAnswersInBank;
using Application.Features.Questions.Query.GetQuestionById;
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
        public async Task<IActionResult> Create([FromForm] AddQuestionCommand command)
        {

            _logger.LogInformation("Fetching course detail for CourseId: {CourseId}", command.QuestionString);
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : NotFound(result.Error);
        }

        //[HttpPatch("{id}")]
        //[Consumes("application/json")]
        //public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuestionCommand command)
        //{
        //    if (id != command.QuestionId)
        //    {
        //        return BadRequest();
        //    }

        //    var result = await _mediator.Send(command);
        //    return result.IsSuccess ? Ok(result) : NotFound(result.Error);
        //}

        [HttpPatch("UpdateQuestion")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateForm([FromForm] UpdateQuestionCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : NotFound(result.Error);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteQuestionCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : NotFound(result.Error);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestionById(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching question detail for QuestionId: {QuestionId}", id);
            var query = new GetQuestionByIdQuery { QuestionId = id };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : NotFound(result.Error);
        }

        [HttpGet("QuestionBank")]
        public async Task<IActionResult> GetAllQuestionsInBank([FromQuery] QuestionRequest questionRequest, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all questions in bank: {BankId}", questionRequest.Id);
            var query = new GetAllQuestionsWithAnswersInBankQuery
            {
                BankId = questionRequest.Id,
                BankType = questionRequest.Type,
                PageNumber = questionRequest.PageNumber
            };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : NotFound(result.Error);
        }

        [HttpGet("exam/{examId}")]
        public async Task<IActionResult> GetAllQuestionsInExam(Guid examId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all questions in exam: {ExamId}", examId);
            var query = new GetAllQuestionsWithAnswersInExamQuery { ExamId = examId };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : NotFound(result.Error);
        }

        [HttpPost("{questionId}/answers")]
        public async Task<IActionResult> AddAnswerToQuestion(Guid questionId, [FromBody] AddAnswerToQuestionCommand command, CancellationToken cancellationToken)
        {
            if (questionId != command.QuestionId)
            {
                return BadRequest("Question ID in route does not match Question ID in body.");
            }

            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

    }
}