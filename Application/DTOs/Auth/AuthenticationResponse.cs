namespace Application.DTOs.Auth
{
    public class AuthenticationResponse
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public string UserRole { get; set; } = string.Empty;
        public bool IsNewUser { get; set; }
        public DateTimeOffset AuthenticatedAt { get; set; }
    }
}
