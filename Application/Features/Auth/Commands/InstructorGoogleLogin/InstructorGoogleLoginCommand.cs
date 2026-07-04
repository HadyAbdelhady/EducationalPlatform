using Application.Features.Auth.DTOs;
using Application.Features.Auth.Commands.StudentGoogleLogin;
using Application.Common;
using MediatR;

namespace Application.Features.Auth.Commands.InstructorGoogleLogin
{
    public class InstructorGoogleLoginCommand : IRequest<Result<AuthenticationResponse>>
    {
        public string IdToken { get; set; } = null!;

        public string Ssn { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string Gender { get; set; } = null!;
        public string ApplicationName { get; set; } = string.Empty;

        public GoogleUserInfo GoogleUserInfo { get; set; } = null!;

    }
}
