namespace Application.DTOs.Center
{
    public class CreateCenterRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public string? LocationMaps { get; set; }
    }

    public class UpdateCenterRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public string? LocationMaps { get; set; }
    }

    public class AssignInstructorToCenterRequest
    {
        public Guid InstructorId { get; set; }
        public List<Guid> EducationYearIds { get; set; } = [];
    }
}
