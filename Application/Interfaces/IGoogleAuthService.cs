using Application.DTOs.Auth;

namespace Application.Interfaces
{
    /// <summary>
    /// Service interface for Google authentication operations.
    /// </summary>
    public interface IGoogleAuthService
    {
        /// <summary>
        /// Validates a Google ID token and extracts user information.
        /// </summary>
        /// <param name="idToken">The Google ID token to validate.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Google user information if token is valid, null otherwise.</returns>
        Task<GoogleUserInfo?> ValidateGoogleTokenAsync(string idToken, CancellationToken cancellationToken = default);
    }
}
