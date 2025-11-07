using Application.DTOs.Auth;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Auth.Commands.StudentGoogleLogin
{
    public class StudentGoogleLoginCommand : IRequest<Result<AuthenticationResponse>>
    {
        public string IdToken { get; set; } = string.Empty;

        public string DeviceId { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public DateOnly DateOfBirth { get; set; }

        public string Gender { get; set; } = string.Empty;

        public string EducationYear { get; set; } = string.Empty;
        public string Ssn { get; set; } = string.Empty;

        public string? LocationMaps { get; set; }
    }
}
