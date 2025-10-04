namespace Application.DTOs.Auth
{
    public class GoogleLoginRequest
    {
        public string IdToken { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
    }
}
