using Application.DTOs.Videos;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Videos.Queries.GetVideoById
{
    public class GetVideoByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetVideoByIdQuery, Result<VideoByUserIdResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<VideoByUserIdResponse>> Handle(GetVideoByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var video = await _unitOfWork.Repository<Video>().GetByIdAsync(request.VideoId, cancellationToken);

                if (video is null)
                    return Result<VideoByUserIdResponse>.FailureStatusCode("Video not found.", ErrorType.NotFound);

                var response = new VideoByUserIdResponse
                {
                    Id = video.Id,
                    Name = video.Name,
                    VideoUrl = video.VideoUrl,
                    Description = video.Description,
                    NumberOfQuestionsSheets = video.Sheets.Where(sh => sh.Type == SheetType.QuestionSheet).Count(),
                    NumberOfTutorialSheets = video.Sheets.Where(sh => sh.Type == SheetType.TutorialSheet).Count(),
                    SectionId = video.SectionId,
                    CreatedAt = video.CreatedAt,
                    UpdatedAt = video.UpdatedAt ?? video.CreatedAt,
                };

                return Result<VideoByUserIdResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<VideoByUserIdResponse>.FailureStatusCode(
                    $"An error occurred while retrieving the video: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }
}
