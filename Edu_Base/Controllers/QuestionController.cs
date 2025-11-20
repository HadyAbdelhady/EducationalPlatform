using Application.Features.Question.Command.AddQuestion;
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddQuestionCommand command)
        {

            _logger.LogInformation("Fetching course detail for CourseId: {CourseId}", command.QuestionString);
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            // Placeholder for your GetQuery
            return Ok();
        }
    }
}