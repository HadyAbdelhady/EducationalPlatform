using Application.DTOs.Auth;
using Application.Features.Auth.Commands.StudentGoogleLogin;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Auth.Commands.InstructorGoogleLogin
{
    public class InstructorGoogleLoginCommand : IRequest<Result<AuthenticationResponse>>
    {
        public string IdToken { get; set; } = string.Empty;

        public string Ssn { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public GoogleUserInfo GoogleUserInfo { get; set; } = null!;

    }
}
