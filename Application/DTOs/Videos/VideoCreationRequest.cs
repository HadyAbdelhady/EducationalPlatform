namespace Application.DTOs.Videos
{
    public class VideoCreationRequest
    {
        public Guid SectionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class VideoBulkCreationRequest
    {
        public string Name { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
