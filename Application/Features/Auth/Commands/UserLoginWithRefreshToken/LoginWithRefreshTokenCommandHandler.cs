using Application.DTOs.Auth;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Auth.Commands.UserLoginWithRefreshToken
{
    public sealed class LoginWithRefreshTokenCommandHandler(IUnitOfWork unitOfWork, IJwtTokenService jwtTokenService)
                                                    : IRequestHandler<LoginWithRefreshTokenCommand, Result<AuthResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

        public async Task<Result<AuthResponse>> Handle(LoginWithRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var refreshToken = await _unitOfWork.GetRepository<IRefreshTokenRepository>().GetRefreshTokenAsync(request.RefreshToken, cancellationToken)
                                                            ?? throw new UnauthorizedAccessException("Invalid refresh token");

                if (refreshToken.ExpiresAt < DateTime.UtcNow)
                {
                    throw new UnauthorizedAccessException("Refresh token has expired");
                }

                var newAccessToken = _jwtTokenService.GenerateToken(refreshToken.User.Id, refreshToken.User.GmailExternal!, request.Role, refreshToken.User.FullName);
                var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

                // Invalidate the old refresh token by removing it
                await _unitOfWork.GetRepository<IRefreshTokenRepository>().DeleteRefreshTokenAsync(refreshToken, cancellationToken);

                // Add the new refresh token
                await _unitOfWork.GetRepository<IRefreshTokenRepository>().AddRefreshTokenAsync(newRefreshToken, request.UserId, cancellationToken);

                // Save all changes in a single transaction
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<AuthResponse>.Success(new AuthResponse
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                });
            }
            catch (UnauthorizedAccessException uaEx)
            {
                return Result<AuthResponse>.FailureStatusCode(uaEx.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<AuthResponse>.FailureStatusCode($"An error occurred while processing the request: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }
}
