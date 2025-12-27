using Application.DTOs.Videos;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Videos.Commands.CreateVideo
{
    public class CreateVideoCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateVideoCommand, Result<VideoResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<VideoResponse>> Handle(CreateVideoCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var Result = 0;
                var SectionId = request.SectionId;
                var video = new Video()
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    VideoUrl = request.VideoUrl,
                    SectionId = request.SectionId,
                    Description = request.Description,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,

                };
                //Check SectionId Is Valid In Table Section 

                //var section = await _unitOfWork.Repository<Domain.Entities.Section>().GetByIdAsync(SectionId.Value);


                //if (section is not null) // Insert Video
                //{

                await _unitOfWork.Repository<Video>().AddAsync(video, cancellationToken);
                Result = await _unitOfWork.SaveChangesAsync(cancellationToken);

                // }
                if (Result > 0)
                {
                    return Result<VideoResponse>.Success(new VideoResponse
                    {
                        VideoId = video.Id,
                        Name = video.Name,
                        VideoUrl = video.VideoUrl,
                        CreatedAt = video.CreatedAt.DateTime
                    });
                }
                return Result<VideoResponse>.FailureStatusCode($"Bad Request {StatusCodes.Status400BadRequest}", ErrorType.BadRequest);

            }
            catch (UnauthorizedAccessException auth)
            {
                return Result<VideoResponse>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<VideoResponse>.FailureStatusCode($"Error creating video: {ex.Message}", ErrorType.Conflict);
            }
        }
    }
}
