namespace Application.DTOs.Sections
{
    public class BulkDeleteSectionRequest
    {
        public Guid CourseId {  get; set; }
        public List<Guid> SectionIds { get; set; } = [];
    }
}
