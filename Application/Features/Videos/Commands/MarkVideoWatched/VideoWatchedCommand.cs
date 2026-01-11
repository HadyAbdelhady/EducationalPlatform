using MediatR;

namespace Application.Features.Videos.Commands.MarkVideoWatched
{
    public record VideoWatchedCommand(Guid VideoId, Guid StudentId) : IRequest<bool>;
}
