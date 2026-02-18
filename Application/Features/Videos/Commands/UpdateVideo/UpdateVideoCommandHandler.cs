using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Videos.Commands.UpdateVideo
{
    public class UpdateVideoCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateVideoCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<string>> Handle(UpdateVideoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var video = await _unitOfWork.Repository<Video>().GetByIdAsync(request.VideoId, cancellationToken)
                                                                    ?? throw new KeyNotFoundException("Video Not Found");

                var section = await _unitOfWork.Repository<Section>().GetByIdAsync(request.SectionId.Value, cancellationToken);


                video.Name = request.Name;
                video.Description = request.Description;
                video.SectionId = section.Id;
                video.VideoUrl = request.VideoUrl;
                video.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<Video>().Update(video);
                var Result = await _unitOfWork.SaveChangesAsync();

                if (Result > 0)
                    return Result<string>.Success("Video Updated Successfully");


                return Result<string>.FailureStatusCode("Failed To Update Video", ErrorType.BadRequest);
            }
            catch (KeyNotFoundException)
            {
                return Result<string>.FailureStatusCode("Video not found", ErrorType.NotFound);
            }
            catch (Exception ex)
            {
                return Result<string>.FailureStatusCode($"An error occurred while updating the video: {ex.Message}", ErrorType.Validation);
            }

        }
    }
}
