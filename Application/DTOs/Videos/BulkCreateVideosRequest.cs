namespace Application.DTOs.Videos
{
    public class BulkCreateVideosRequest
    {
        public Guid SectionId { get; set; }
        public List<VideoBulkCreationRequest> Videos { get; set; } = [];
    }
}
