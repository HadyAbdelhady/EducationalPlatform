namespace Application.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<bool?> ValidateGoogleTokenAsync(string idToken, CancellationToken cancellationToken = default);
    }
}
