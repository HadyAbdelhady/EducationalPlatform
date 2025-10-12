using Application.DTOs.Auth;
using MediatR;

namespace Application.Features.Auth.Commands.InstructorGoogleSignUp
{
    public record InstructorGoogleSignUpCommand : IRequest<AuthenticationResponse>
    {
        public string IdToken { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Ssn { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string EducationYear { get; set; } = string.Empty;
        public string? LocationMaps { get; set; }
    }
}
