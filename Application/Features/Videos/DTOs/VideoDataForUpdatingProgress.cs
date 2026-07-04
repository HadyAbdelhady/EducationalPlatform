using Domain.Entities;

namespace Application.Features.Videos.DTOs
{
    public class VideoDataForUpdatingProgress
    {
        public Video Video { get; set; } = null!;
        public StudentCourse? StudentCourse { get; set; }
        public int NumberOfVideosInCourse { get; set; }
        public int NumberOfVideosInSection { get; set; }
        public StudentSection? StudentSection { get; set; }
    }
}
