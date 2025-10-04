using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auth.Commands.StudentGoogleLogin
{
    /// <summary>
    /// Handles the student Google login command.
    /// Validates the Google token, creates or updates the student account, and returns authentication response.
    /// </summary>
    public class StudentGoogleLoginCommandHandler(
        IGoogleAuthService googleAuthService,
        IUserRepository userRepository) : IRequestHandler<StudentGoogleLoginCommand, AuthenticationResponse>
    {
        private readonly IGoogleAuthService _googleAuthService = googleAuthService;
        private readonly IUserRepository _userRepository = userRepository;

        /// <summary>
        /// Handles the student Google login process.
        /// </summary>
        /// <param name="request">The student Google login command.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Authentication response with user information.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when Google token is invalid.</exception>
        public async Task<AuthenticationResponse> Handle(StudentGoogleLoginCommand request, CancellationToken cancellationToken)
        {
            // Validate Google ID token
            var googleUserInfo = await _googleAuthService.ValidateGoogleTokenAsync(request.IdToken, cancellationToken);
            
            if (googleUserInfo == null || !googleUserInfo.EmailVerified)
            {
                throw new UnauthorizedAccessException("Invalid Google token or email not verified.");
            }

            // Check if user already exists
            var existingUser = await _userRepository.GetByGoogleEmailAsync(googleUserInfo.Email, cancellationToken);
            
            bool isNewUser = existingUser == null;
            User user;

            if (existingUser == null)
            {
                // Create new user and student
                user = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = googleUserInfo.FullName,
                    Ssn = request.Ssn, // Will be updated later if needed
                    PhoneNumber = request.PhoneNumber,
                    GmailExternal = googleUserInfo.Email,
                    PersonalPictureUrl = googleUserInfo.PictureUrl,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    EducationYear = request.EducationYear,
                    LocationMaps = request.LocationMaps,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow,
                    IsDeleted = false
                };

                var student = new Student
                {
                    UserId = user.Id,
                    DeviceId = request.DeviceId,
                    ScreenshotTrial = 0
                };

                user.Student = student;
                
                await _userRepository.CreateAsync(user, cancellationToken);
            }
            else
            {
                // Update existing user
                user = existingUser;
                user.UpdatedAt = DateTimeOffset.UtcNow;
                user.PersonalPictureUrl = googleUserInfo.PictureUrl ?? user.PersonalPictureUrl;
                
                // Update student device if different
                if (user.Student != null && user.Student.DeviceId != request.DeviceId)
                {
                    user.Student.DeviceId = request.DeviceId;
                }

                await _userRepository.UpdateAsync(user, cancellationToken);
            }

            await _userRepository.SaveChangesAsync(cancellationToken);

            // Return authentication response
            return new AuthenticationResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.GmailExternal ?? string.Empty,
                ProfilePictureUrl = user.PersonalPictureUrl,
                UserRole = "Student",
                IsNewUser = isNewUser,
                AuthenticatedAt = DateTimeOffset.UtcNow
            };
        }
    }
}
