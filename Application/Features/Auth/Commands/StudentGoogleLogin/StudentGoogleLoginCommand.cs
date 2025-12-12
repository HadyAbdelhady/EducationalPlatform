using Application.DTOs.Auth;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Auth.Commands.StudentGoogleLogin
{
    public class StudentGoogleLoginCommand : IRequest<Result<AuthenticationResponse>>
    {
        public string DeviceId { get; set; } = string.Empty;
        public string ParentPhoneNumber { get; set; } = string.Empty;
        public Guid EducationYearId { get; set; }
        public string Ssn { get; set; } = string.Empty;
        public string? LocationMaps { get; set; }
        public GoogleUserInfo GoogleUserInfo { get; set; } = null!;
    }

    public class GoogleUserInfo
    {
        //public string GoogleId { get; set; } = string.Empty;
        public string IdToken { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PictureUrl { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }

        //public bool EmailVerified { get; set; }
    }
}
