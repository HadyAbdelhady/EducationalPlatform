namespace Application.DTOs.EducationYear
{
    public class EducationYearResponse
    {
        public Guid Id { get; set; }
        public string EducationYearName { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
