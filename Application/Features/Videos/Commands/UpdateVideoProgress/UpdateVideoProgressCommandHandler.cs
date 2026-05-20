using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Videos.Commands.UpdateVideoProgress
{
    public class UpdateVideoProgressCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateVideoProgressCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<bool> Handle(UpdateVideoProgressCommand request, CancellationToken cancellationToken)
        {
            if (request.Progress is < 0 or > 100)
            {
                return false;
            }

            var now = EgyptTime.Now;

            var video = await _unitOfWork.Repository<Video>()
                                         .FirstOrDefaultAsync(v => v.Id == request.VideoId, cancellationToken,
                                            v => v.StudentVideos,
                                            v => v.Section!,
                                            v => v.Section!.StudentSections,
                                            v => v.Section!.Course!,
                                            v => v.Section!.Course!.StudentCourses);

            if (video == null)
            {
                return false;
            }

            var studentSection = video.Section?.StudentSections.FirstOrDefault(ss => ss.StudentId == request.StudentId);
            var studentCourse = video.Section?.Course?.StudentCourses.FirstOrDefault(sc => sc.StudentId == request.StudentId);

            if (studentCourse == null && studentSection == null)
            {
                return false;
            }

            var studentVideo = video.StudentVideos.FirstOrDefault(sv => sv.StudentId == request.StudentId);
            var requestedProgress = request.Progress;
            var previousProgress = studentVideo?.Progress ?? 0;
            var nextProgress = Math.Max(previousProgress, requestedProgress);
            var transitionedToCompleted = previousProgress < 100 && nextProgress == 100;

            if (studentVideo == null)
            {
                studentVideo = new StudentVideo
                {
                    StudentId = request.StudentId,
                    VideoId = request.VideoId,
                    Progress = nextProgress,
                    WatchedAt = nextProgress == 100 ? now : default,
                    UpdatedAt = now
                };

                video.StudentVideos.Add(studentVideo);
            }
            else
            {
                studentVideo.Progress = nextProgress;
                studentVideo.UpdatedAt = now;
                if (nextProgress == 100 && studentVideo.WatchedAt == default)
                {
                    studentVideo.WatchedAt = now;
                }
            }

            if (transitionedToCompleted)
            {
                if (studentCourse != null)
                {
                    studentCourse.NumberOfCourseVideosWatched++;
                    studentCourse.Progress = CalculateProgress(
                        studentCourse.NumberOfCourseVideosWatched,
                        video.Section?.Course?.NumberOfVideos ?? 0);
                    studentCourse.UpdatedAt = now;
                }

                if (studentSection != null)
                {
                    studentSection.NumberOfSectionVideosWatched++;
                    studentSection.Progress = CalculateProgress(
                        studentSection.NumberOfSectionVideosWatched,
                        video.Section?.NumberOfVideos ?? 0);
                    studentSection.UpdatedAt = now;
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        private static decimal CalculateProgress(int watched, int total)
        {
            return total == 0 ? 0 : Math.Round((decimal)watched / total, 4);
        }
    }
}
