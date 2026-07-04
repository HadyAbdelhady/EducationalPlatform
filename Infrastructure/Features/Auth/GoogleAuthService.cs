using Application.Features.Auth.DTOs;
using Application.Common.Interfaces;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;

namespace Infrastructure.Common.Services
{
    public class GoogleAuthSettings
    {
        public string[] ClientId { get; set; } = [];
    }
    public class GoogleAuthService(IOptions<GoogleAuthSettings> googleAuthOptions) : IGoogleAuthService
    {
        private readonly GoogleAuthSettings _settings = googleAuthOptions.Value;

        public async Task<bool?> ValidateGoogleTokenAsync(string idToken, CancellationToken cancellationToken = default)
        {
            try
            {
                if (_settings.ClientId == null || _settings.ClientId.Length == 0)
                    throw new InvalidOperationException("Google Client IDs not configured.");

                var validationSettings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = _settings.ClientId
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, validationSettings);

                if (payload == null)
                {
                    return false;
                }

                return true;
            }
            catch (InvalidJwtException ex)
            {
                throw new UnauthorizedAccessException($"Invalid JWT token: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException($"Error validating Google token: {ex.Message}");
            }
        }
    }
}
