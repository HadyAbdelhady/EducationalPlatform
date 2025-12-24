namespace Application.DTOs.Sections
{
    public class GetSectionByIDResponse
    {
        public Guid SectionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int NumberOfVideos { get; set; }
        public decimal? Rating { get; set; }
        public int NumberOfQuestionSheets { get; set; }
        public Guid CourseId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}

