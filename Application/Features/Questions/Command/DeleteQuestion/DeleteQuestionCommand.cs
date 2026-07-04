using MediatR;
using Application.Common;

namespace Application.Features.Questions.Command.DeleteQuestion
{
    public record DeleteQuestionCommand(Guid QuestionId) : IRequest<Result<Guid>>;
}