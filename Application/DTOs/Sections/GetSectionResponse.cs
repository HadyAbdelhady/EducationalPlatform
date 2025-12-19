namespace Application.DTOs.Sections
{
    public class GetSectionResponse
    {
        public Guid SectionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
