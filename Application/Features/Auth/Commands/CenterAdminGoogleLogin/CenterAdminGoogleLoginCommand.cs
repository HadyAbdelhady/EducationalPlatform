using Application.DTOs.Auth;
using Application.Features.Auth.Commands.StudentGoogleLogin;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Auth.Commands.CenterAdminGoogleLogin
{
    public class CenterAdminGoogleLoginCommand : IRequest<Result<AuthenticationResponse>>
    {
        public string Ssn { get; set; } = string.Empty;
        public string? LocationMaps { get; set; }
        public Guid CenterId { get; set; }
        public GoogleUserInfo GoogleUserInfo { get; set; } = null!;
    }
}
