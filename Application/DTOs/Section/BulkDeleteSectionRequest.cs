namespace Application.DTOs.Section
{
    public class BulkDeleteSectionRequest
    {
        public Guid CourseId {  get; set; }
        public List<Guid> SectionIds { get; set; } = [];
    }
}
