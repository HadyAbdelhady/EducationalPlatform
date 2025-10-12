using Application.DTOs.Auth;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Auth.Commands.UserLoginWithRefreshToken
{
    public sealed class LoginWithRefreshTokenHandler(IRefreshTokenRepository refreshTokenRepository, IJwtTokenService jwtTokenService)
                                                    :IRequestHandler<LoginWithRefreshTokenCommand, AuthResponse>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

        public async Task<AuthResponse> Handle(LoginWithRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(request.RefreshToken, cancellationToken)
                                                        ?? throw new UnauthorizedAccessException("Invalid refresh token");

            if (refreshToken.ExpiresAt < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Refresh token has expired");
            }

            var newAccessToken = _jwtTokenService.GenerateToken(refreshToken.User.Id, refreshToken.User.GmailExternal!, request.Role, refreshToken.User.FullName);
            var newRefreshToken = await _jwtTokenService.GenerateRefreshToken(request.UserId, cancellationToken);

            // Invalidate the old refresh token by removing it
            await _refreshTokenRepository.DeleteRefreshTokenAsync(refreshToken, cancellationToken);

            return new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
    }
}
