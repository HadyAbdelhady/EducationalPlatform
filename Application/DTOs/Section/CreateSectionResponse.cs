namespace Application.DTOs.Section
{
    public class CreateSectionResponse
    {
        public Guid SectionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
