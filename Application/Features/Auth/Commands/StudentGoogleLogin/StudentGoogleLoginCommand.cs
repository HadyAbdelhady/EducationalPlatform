using Application.DTOs.Auth;
using MediatR;

namespace Application.Features.Auth.Commands.StudentGoogleLogin
{
    /// <summary>
    /// Command for authenticating a student using Google OAuth.
    /// </summary>
    public class StudentGoogleLoginCommand : IRequest<AuthenticationResponse>
    {
        /// <summary>
        /// Google ID token received from Google Sign-In.
        /// </summary>
        public string IdToken { get; set; } = string.Empty;

        /// <summary>
        /// Device ID for the student (required for tracking).
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>
        /// Phone number for the student account.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Date of birth for the student.
        /// </summary>
        public DateOnly DateOfBirth { get; set; }

        /// <summary>
        /// Gender of the student.
        /// </summary>
        public string Gender { get; set; } = string.Empty;

        /// <summary>
        /// Education year of the student.
        /// </summary>
        public string EducationYear { get; set; } = string.Empty;

        /// <summary>
        /// Optional location maps URL.
        /// </summary>
        public string? LocationMaps { get; set; }
    }
}
