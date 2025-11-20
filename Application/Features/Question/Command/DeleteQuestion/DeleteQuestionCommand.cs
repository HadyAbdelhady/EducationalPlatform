using MediatR;
using Application.ResultWrapper;

namespace Application.Features.Question.Command.DeleteQuestion
{
    public record DeleteQuestionCommand(Guid QuestionId) : IRequest<Result<Guid>>;
}