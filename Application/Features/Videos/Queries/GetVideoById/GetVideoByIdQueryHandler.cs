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
            // Load the video with its sheets (no StudentVideos filter here)
            var video = await _unitOfWork.Repository<Video>()
                .FirstOrDefaultAsync(v => v.Id == request.VideoId, cancellationToken,
                    v => v.Sheets);

            if (video is null)
                return Result<VideoByUserIdResponse>.FailureStatusCode("Video not found.", ErrorType.NotFound);

            // Access control: if a student is making the request, verify enrollment
            int? progress = null;
            if (request.StudentId is not null)
            {
                var enrollmentRepo = unitOfWork.GetRepository<IStudentEnrollmentRepository>();
                var isEnrolled = await enrollmentRepo.CanStudentAccessSectionContentAsync(
                    request.StudentId.Value,
                    video.SectionId,
                    cancellationToken);

                if (!isEnrolled)
                    return Result<VideoByUserIdResponse>.FailureStatusCode(
                        "You are not enrolled in the course or section that contains this video.",
                        ErrorType.Forbidden);

                // Load progress from student_videos (row may not exist yet if student never updated progress)
                var studentVideo = await _unitOfWork.Repository<StudentVideo>()
                    .FirstOrDefaultAsync(
                        sv => sv.StudentId == request.StudentId.Value && sv.VideoId == request.VideoId,
                        cancellationToken);

                progress = studentVideo?.Progress;
            }

            var response = new VideoByUserIdResponse
            {
                Id = video.Id,
                Name = video.Name,
                VideoUrl = video.VideoUrl,
                Description = video.Description,
                Progress = progress ?? 0,
                NumberOfQuestionsSheets = video.Sheets.Count(sh => sh.Type == SheetType.QuestionSheet),
                NumberOfTutorialSheets = video.Sheets.Count(sh => sh.Type == SheetType.TutorialSheet),
                SectionId = video.SectionId,
                CreatedAt = video.CreatedAt,
                UpdatedAt = video.UpdatedAt ?? video.CreatedAt,
            };

            return Result<VideoByUserIdResponse>.Success(response);
        }
    }
}
