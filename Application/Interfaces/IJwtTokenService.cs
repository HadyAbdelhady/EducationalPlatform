using Domain.Entities;
using System.Security.Claims;

namespace Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(Guid userId, string email, string role, string fullName);
        ClaimsPrincipal? ValidateToken(string token);
        public Task<string> GenerateRefreshToken(Guid UserId,CancellationToken cancellationToken);

        //Task<string> AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
        //Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken);
    }
}
