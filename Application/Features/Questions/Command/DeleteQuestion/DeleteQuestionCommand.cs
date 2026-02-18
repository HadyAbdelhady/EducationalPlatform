using MediatR;
using Application.ResultWrapper;

namespace Application.Features.Questions.Command.DeleteQuestion
{
    public record DeleteQuestionCommand(Guid QuestionId) : IRequest<Result<Guid>>;
}