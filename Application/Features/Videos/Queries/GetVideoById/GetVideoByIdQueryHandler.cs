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
                Video? video = new();
                if (request.StudentId is not null)
                    video = _unitOfWork.Repository<Video>().Find(v => v.Id == request.VideoId &&
                                                                v.StudentVideos.Any(sv => sv.StudentId == request.StudentId),
                                                                cancellationToken,
                                                         sv => sv.StudentVideos)
                                                            .FirstOrDefault();
                else
                {
                    video = _unitOfWork.Repository<Video>().Find(v => v.Id == request.VideoId,
                                                               cancellationToken)
                                                           .FirstOrDefault();

                }

                if (video is null)
                    return Result<VideoByUserIdResponse>.FailureStatusCode("Video not found.", ErrorType.NotFound);

                var response = new VideoByUserIdResponse
                {
                    Id = video.Id,
                    Name = video.Name,
                    VideoUrl = video.VideoUrl,
                    Description = video.Description,
                    Progress = video.StudentVideos.Select(s => s.Progress).FirstOrDefault(),
                    NumberOfQuestionsSheets = video.Sheets.Count(sh => sh.Type == SheetType.QuestionSheet),
                    NumberOfTutorialSheets = video.Sheets.Count(sh => sh.Type == SheetType.TutorialSheet),
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
