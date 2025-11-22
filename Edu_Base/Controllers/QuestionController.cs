using Application.Features.Question.Command.AddQuestion;
using Application.Features.Question.Command.DeleteQuestion;
using Application.Features.Question.Command.UpdateQuestion;
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
    }
}