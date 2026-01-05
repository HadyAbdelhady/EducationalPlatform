namespace Application.DTOs.Sections
{
    public class SectionUpdateResponse
    {
        public Guid SectionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }
}
