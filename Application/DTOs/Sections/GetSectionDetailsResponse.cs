using Domain.Entities;

namespace Application.DTOs.Sections
{   
    public class GetSectionDetailsResponse
    {
        public Guid SectionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int NumberOfVideos { get; set; }
        public int NumberOfSectionVideosWatched { get; set; }
        public decimal? Rating { get; set; }
        public int NumberOfQuestionSheets { get; set; }
        public Guid CourseId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public List<VideoData>? Videos { get; set; }
    }

    public sealed class SectionDetailsQueryModel
    {
        public Section Section { get; init; } = default!;
        public StudentSectionData? StudentSection { get; init; }
        public List<VideoData> Videos { get; init; } = [];
    }

    public sealed class StudentSectionData
    {
        public DateTimeOffset EnrolledAt { get; init; }
        public int NumberOfSectionVideosWatched { get; init; }
    }

    public sealed class VideoData
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string VideoUrl { get; init; } = string.Empty;
        public decimal? Rating { get; init; }
        public StudentVideoData? StudentVideo { get; init; }
    }

    public sealed class StudentVideoData
    {
        public DateTimeOffset? WatchedAt { get; init; }
        public int Progress { get; init; }
    }

}

