using Application.DTOs.Auth;
using Application.Interfaces;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IConfiguration _configuration;

        public GoogleAuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<GoogleUserInfo?> ValidateGoogleTokenAsync(string idToken, CancellationToken cancellationToken = default)
        {
            try
            {
                var clientId = _configuration["Authentication:Google:ClientId"];
                
                if (string.IsNullOrEmpty(clientId))
                {
                    throw new InvalidOperationException("Google Client ID is not configured.");
                }

                // Validate the token with Google
                var validationSettings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { clientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, validationSettings);

                if (payload == null)
                {
                    return null;
                }

                // Extract user information from the validated token
                return new GoogleUserInfo
                {
                    GoogleId = payload.Subject,
                    Email = payload.Email,
                    FullName = payload.Name ?? string.Empty,
                    PictureUrl = payload.Picture,
                    EmailVerified = payload.EmailVerified
                };
            }
            catch (InvalidJwtException)
            {
                // Token is invalid
                return null;
            }
            catch (Exception)
            {
                // Log the exception in a real application
                return null;
            }
        }
    }
}
