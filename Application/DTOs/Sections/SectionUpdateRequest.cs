namespace Application.DTOs.Sections
{
    public class SectionUpdateRequest
    {
        public Guid SectionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        //[Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        //public decimal Price { get; set; }
        public Guid CourseId { get; set; }
    }
}
