using Application.DTOs.Videos;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Videos.Commands.CreateVideo
{
    public class BulkCreateVideosCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<BulkCreateVideosCommand, Result<List<VideoCreationResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<List<VideoCreationResponse>>> Handle(BulkCreateVideosCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var videoRepo = _unitOfWork.Repository<Video>();
                var sectionRepo = _unitOfWork.Repository<Section>();

                List<Video> videosTobeAdded = [];
                var responses = new List<VideoCreationResponse>();

                var Result = 0;

                foreach (var video in request.Videos)
                {
                    var newVideo = new Video()
                    {
                        Id = Guid.NewGuid(),
                        Name = video.Name,
                        Description = video.Description,
                        VideoUrl = video.VideoUrl,
                        SectionId = video.SectionId,

                    };
                    videosTobeAdded.Add(newVideo);

                }
                await videoRepo.AddRangeAsync(videosTobeAdded, cancellationToken);

                foreach (var video in videosTobeAdded)
                {
                    responses.Add(new VideoCreationResponse()
                    {
                        VideoId = video.Id,
                        Name = video.Name,
                        CreatedAt = video.CreatedAt.UtcDateTime,
                        Description = video.Description,
                        VideoUrl = video.VideoUrl,

                    });

                    var section = await sectionRepo.GetByIdAsync(video.SectionId.Value);
                    if (section is not null)
                    {
                        section.NumberOfVideos++;
                        section.UpdatedAt = DateTimeOffset.UtcNow;
                        sectionRepo.Update(section);
                    }
                }

                Result = await _unitOfWork.SaveChangesAsync(cancellationToken);
                if (Result > 0)
                {
                    return Result<List<VideoCreationResponse>>.Success(responses);

                }
                return Result<List<VideoCreationResponse>>.FailureStatusCode("Error While Inserting Videos", ErrorType.BadRequest);

            }
            catch (Exception ex)
            {
                return Result<List<VideoCreationResponse>>
                    .FailureStatusCode($"Error in bulk create for videos: {ex.Message}", ErrorType.InternalServerError);
            }

        }
    }
}
