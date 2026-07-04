namespace Application.Features.Videos.DTOs
{
    public class BulkCreateVideosRequest
    {
        public Guid SectionId { get; set; }
        public List<VideoBulkCreationRequest> Videos { get; set; } = [];
    }
}
