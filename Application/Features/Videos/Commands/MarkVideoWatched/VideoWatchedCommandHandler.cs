using Application.Features.Videos.Commands.UpdateVideoProgress;
using MediatR;

namespace Application.Features.Videos.Commands.MarkVideoWatched
{
    public class VideoWatchedCommandHandler(IMediator mediator) : IRequestHandler<VideoWatchedCommand, bool>
    {
        private readonly IMediator _mediator = mediator;

        public async Task<bool> Handle(VideoWatchedCommand request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new UpdateVideoProgressCommand(request.VideoId, request.StudentId, 100), cancellationToken);
        }
    }
}
