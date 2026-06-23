using Application.DTOs.Auth;
using Application.Features.Auth.Commands.StudentGoogleLogin;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Auth.Commands.InstructorGoogleLogin
{
    public class InstructorGoogleLoginCommand : IRequest<Result<AuthenticationResponse>>
    {
        public string IdToken { get; set; } = null!;

        public string Ssn { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string Gender { get; set; } = null!;

        public GoogleUserInfo GoogleUserInfo { get; set; } = null!;

    }
}
