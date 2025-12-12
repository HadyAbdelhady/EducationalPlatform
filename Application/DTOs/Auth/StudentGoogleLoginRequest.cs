namespace Application.DTOs.Auth
{
    public class StudentGoogleLoginRequest
    {
        public string IdToken { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
        public string Ssn { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PictureUrl { get; set; }
        public string ParentPhoneNumber { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public Guid EducationYearId { get; set; }
        public string? LocationMaps { get; set; }
    }
}
