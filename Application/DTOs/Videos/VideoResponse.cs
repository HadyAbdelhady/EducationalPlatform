namespace Application.DTOs.Videos
{
    public class VideoResponse
    {
        public Guid VideoId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public decimal? Rating { get; set; }
        public VideoProgressData? ProgressData { get; set; }
    }
    public class VideoProgressData
    {

        public DateTimeOffset? EnrolledAt { get; set; }
        public DateTimeOffset? WatchedAt { get; set; }
        public int? Progress { get; set; }
    }
}
