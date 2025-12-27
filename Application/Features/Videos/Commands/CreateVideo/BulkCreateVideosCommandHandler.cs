using Application.DTOs.Videos;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using Domain.Events;
using MediatR;

namespace Application.Features.Videos.Commands.CreateVideo
{
    public class BulkCreateVideosCommandHandler(IUnitOfWork unitOfWork, IMediator mediator) : IRequestHandler<BulkCreateVideosCommand, Result<List<VideoResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMediator _mediator = mediator;

        public async Task<Result<List<VideoResponse>>> Handle(BulkCreateVideosCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var videoRepo = _unitOfWork.Repository<Video>();

                List<Video> videosTobeAdded = [];
                var responses = new List<VideoResponse>();

                var Result = 0;

                foreach (var video in request.Videos)
                {
                    var newVideo = new Video()
                    {
                        Id = Guid.NewGuid(),
                        Name = video.Name,
                        Description = video.Description,
                        VideoUrl = video.VideoUrl,
                        SectionId = request.SectionId,
                    };
                    videosTobeAdded.Add(newVideo);

                }
                await videoRepo.AddRangeAsync(videosTobeAdded, cancellationToken);

                foreach (var video in videosTobeAdded)
                {
                    responses.Add(new VideoResponse()
                    {
                        VideoId = video.Id,
                        Name = video.Name,
                        CreatedAt = video.CreatedAt.UtcDateTime,
                        VideoUrl = video.VideoUrl,
                    });
                }

                await _mediator.Publish(new VideoAddedEvent(request.SectionId, videosTobeAdded.Count), cancellationToken);
                Result = await _unitOfWork.SaveChangesAsync(cancellationToken);
                if (Result > 0)
                {
                    return Result<List<VideoResponse>>.Success(responses);

                }
                return Result<List<VideoResponse>>.FailureStatusCode("Error While Inserting Videos", ErrorType.BadRequest);

            }
            catch (Exception ex)
            {
                return Result<List<VideoResponse>>
                    .FailureStatusCode($"Error in bulk create for videos: {ex.Message}", ErrorType.InternalServerError);
            }

        }
    }
}
