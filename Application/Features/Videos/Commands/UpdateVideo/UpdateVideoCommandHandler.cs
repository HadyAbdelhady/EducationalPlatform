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
                Section? section = null;
                var video = await _unitOfWork.Repository<Video>().GetByIdAsync(request.VideoId, cancellationToken)
                                                                    ?? throw new KeyNotFoundException("Video Not Found");

                if (request.SectionId.HasValue)
                    section = await _unitOfWork.Repository<Section>().GetByIdAsync(request.SectionId.Value, cancellationToken);

                if (section == null)
                    return Result<string>.FailureStatusCode("Section not found", ErrorType.NotFound);

                if (!string.IsNullOrEmpty(request.Name))
                    video.Name = request.Name;
                
                if (!string.IsNullOrEmpty(request.Description))
                    video.Description = request.Description;
                
                if (request.SectionId.HasValue)
                    video.SectionId = section.Id;
                
                if (!string.IsNullOrEmpty(request.VideoUrl))
                    video.VideoUrl = request.VideoUrl;

                video.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<Video>().Update(video);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<string>.Success("Video updated successfully");
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
