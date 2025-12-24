namespace Application.DTOs.Videos
{
    public class VideoResponse
    {
        public Guid VideoId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;

        public DateTimeOffset CreatedAt { get; set; }
        public string? Description { get; set; }
    }
}
