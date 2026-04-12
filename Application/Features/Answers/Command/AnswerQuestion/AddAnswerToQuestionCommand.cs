using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Answers.Command.AnswerQuestion
{
    public record AddAnswerToQuestionCommand : IRequest<Result<Guid>>
    {
        public Guid QuestionId { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public string? Explanation { get; set; }
    }
}
