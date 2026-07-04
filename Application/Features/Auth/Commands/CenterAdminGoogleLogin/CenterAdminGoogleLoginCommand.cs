using Application.Features.Auth.DTOs;
using Application.Features.Auth.Commands.StudentGoogleLogin;
using Application.Common;
using MediatR;

namespace Application.Features.Auth.Commands.CenterAdminGoogleLogin
{
    public class CenterAdminGoogleLoginCommand : IRequest<Result<AuthenticationResponse>>
    {
        public string Ssn { get; set; } = null!;
        public string? LocationMaps { get; set; }
        public Guid CenterId { get; set; }
        public GoogleUserInfo GoogleUserInfo { get; set; } = null!;
    }
}
