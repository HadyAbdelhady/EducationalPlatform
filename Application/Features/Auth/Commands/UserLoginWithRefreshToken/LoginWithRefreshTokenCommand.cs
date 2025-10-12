using Application.DTOs.Auth;
using MediatR;

namespace Application.Features.Auth.Commands.UserLoginWithRefreshToken
{
    public record LoginWithRefreshTokenCommand : IRequest<AuthResponse>
    {
        public string RefreshToken { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public Guid UserId{ get; set; }
    }
}