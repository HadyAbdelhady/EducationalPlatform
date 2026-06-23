namespace Application.DTOs.Center
{
    public class CenterResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public string? LocationMaps { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public List<CenterInstructorDto> Instructors { get; set; } = [];
    }

    public class CenterInstructorDto
    {
        public Guid InstructorId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public List<CenterEducationYearDto> EducationYears { get; set; } = [];
    }

    public class CenterEducationYearDto
    {
        public Guid EducationYearId { get; set; }
        public string EducationYearName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class CenterSummaryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public int InstructorCount { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
