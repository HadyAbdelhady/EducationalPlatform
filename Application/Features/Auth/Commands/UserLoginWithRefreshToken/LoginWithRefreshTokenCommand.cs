using Application.DTOs.Auth;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Auth.Commands.UserLoginWithRefreshToken
{
    public record LoginWithRefreshTokenCommand : IRequest<Result<AuthResponse>>
    {
        public string RefreshToken { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}