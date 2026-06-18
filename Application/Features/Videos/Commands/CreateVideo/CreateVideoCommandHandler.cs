using Application.DTOs.Videos;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using Domain.Events;
using MediatR;

namespace Application.Features.Videos.Commands.CreateVideo
{
    public class CreateVideoCommandHandler(IUnitOfWork unitOfWork, IMediator mediator) : IRequestHandler<CreateVideoCommand, Result<VideoResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMediator _mediator = mediator;

        public async Task<Result<VideoResponse>> Handle(CreateVideoCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var SectionId = request.SectionId;
                var video = new Video()
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    VideoUrl = request.VideoUrl,
                    SectionId = request.SectionId,
                    Description = request.Description,
                    CreatedAt = EgyptTime.UtcNow,
                    UpdatedAt = EgyptTime.UtcNow,

                };

                var section = await _unitOfWork.Repository<Section>().GetByIdAsync(SectionId, cancellationToken);

                if (section is not null)
                {
                    await _unitOfWork.Repository<Video>().AddAsync(video, cancellationToken);
                    await _mediator.Publish(new VideoAddedEvent(request.SectionId, 1), cancellationToken);

                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    return Result<VideoResponse>.Success(new VideoResponse
                    {
                        VideoId = video.Id,
                        Name = video.Name,
                        VideoUrl = video.VideoUrl,
                        CreatedAt = video.CreatedAt.DateTime
                    });
                }
                return Result<VideoResponse>.FailureStatusCode("Failed to save the video. No changes were persisted to the database.", ErrorType.BadRequest);

            }
            catch (UnauthorizedAccessException auth)
            {
                return Result<VideoResponse>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<VideoResponse>.FailureStatusCode($"Error creating video: {ex.Message} {ex.InnerException?.Message}", ErrorType.InternalServerError);
            }
        }
    }
}
