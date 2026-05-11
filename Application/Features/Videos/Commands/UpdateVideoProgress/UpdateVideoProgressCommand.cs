using MediatR;

namespace Application.Features.Videos.Commands.UpdateVideoProgress
{
    public record UpdateVideoProgressCommand(Guid VideoId, Guid StudentId, int Progress) : IRequest<bool>;
}
