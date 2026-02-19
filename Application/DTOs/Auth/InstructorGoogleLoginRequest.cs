namespace Application.DTOs.Auth
{
    public class InstructorGoogleLoginRequest
    {
        public string IdToken { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PictureUrl { get; set; }
        public string Ssn { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
    }
}
