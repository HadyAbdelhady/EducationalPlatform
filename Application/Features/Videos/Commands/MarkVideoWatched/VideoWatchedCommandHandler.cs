using Application.DTOs.Videos;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Videos.Commands.MarkVideoWatched
{
    public class VideoWatchedCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<VideoWatchedCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<bool> Handle(VideoWatchedCommand Request, CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.UtcNow;

            var videoData = _unitOfWork.Repository<Video>()
                                        .Find(v => v.Id == Request.VideoId, cancellationToken)
                                        .Select(v => new VideoDataForUpdatingProgress
                                        {
                                            Video = v,
                                            StudentSection = v.Section!.StudentSections.FirstOrDefault(ss => ss.StudentId == Request.StudentId),
                                            NumberOfVideosInSection = v.Section!.NumberOfVideos,
                                            StudentCourse = v.Section!.Course!.StudentCourses.FirstOrDefault(sc => sc.StudentId == Request.StudentId),
                                            NumberOfVideosInCourse = v.Section!.Course!.NumberOfVideos
                                        }).FirstOrDefault();

            if (videoData == null) return false;

            // If enrolled in course, update both
            if (videoData.StudentCourse != null)
            {
                videoData.StudentCourse.NumberOfCourseVideosWatched++;
                videoData.StudentCourse.Progress = CalculateProgress(videoData.StudentCourse.NumberOfCourseVideosWatched,
                                                                        videoData.NumberOfVideosInCourse
                );
                videoData.StudentCourse.UpdatedAt = now;

                videoData.StudentSection!.NumberOfSectionVideosWatched++;
                videoData.StudentSection!.Progress = CalculateProgress(videoData.StudentSection!.NumberOfSectionVideosWatched,
                                                                         videoData.NumberOfVideosInSection
                );
                videoData.StudentSection.UpdatedAt = now;

            }
            // If enrolled in section only
            else if (videoData.StudentSection != null)
            {
                videoData.StudentSection.NumberOfSectionVideosWatched++;
                videoData.StudentSection.Progress = CalculateProgress(videoData.StudentSection.NumberOfSectionVideosWatched,
                                                                        videoData.NumberOfVideosInSection);
                videoData.StudentSection.UpdatedAt = now;
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
