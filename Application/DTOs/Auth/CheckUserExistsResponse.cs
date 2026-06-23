namespace Application.DTOs.Auth
{
    public class CheckUserExistsResponse
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
