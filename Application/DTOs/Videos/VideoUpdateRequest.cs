namespace Application.DTOs.Videos
{
    public class VideoUpdateRequest
    {
        public Guid VideoId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;

        public string? Description { get; set; }
        public Guid? SectionId { get; set; }
    }
}
