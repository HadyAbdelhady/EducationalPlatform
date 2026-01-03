namespace Application.DTOs.Sections
{
    public class BulkCreateSectionRequest
    {
        public Guid CourseId { get; set; }
        public List<CreateSectionRequest> Sections { get; set; } = [];
    }
}
