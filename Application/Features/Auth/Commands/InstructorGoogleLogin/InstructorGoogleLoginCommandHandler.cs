using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auth.Commands.InstructorGoogleLogin
{
    /// <summary>
    /// Handles the instructor Google login command.
    /// Validates the Google token, creates or updates the instructor account, and returns authentication response.
    /// </summary>
    public class InstructorGoogleLoginCommandHandler(
        IGoogleAuthService googleAuthService,
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService) : IRequestHandler<InstructorGoogleLoginCommand, AuthenticationResponse>
    {
        private readonly IGoogleAuthService _googleAuthService = googleAuthService;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

        /// <summary>
        /// Handles the instructor Google login process.
        /// </summary>
        /// <param name="request">The instructor Google login command.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Authentication response with user information.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when Google token is invalid.</exception>
        public async Task<AuthenticationResponse> Handle(InstructorGoogleLoginCommand request, CancellationToken cancellationToken)
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
                // Create new user and instructor
                user = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = googleUserInfo.FullName,
                    Ssn = string.Empty, // Will be updated later if needed
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

                var instructor = new Instructor
                {
                    UserId = user.Id
                };

                user.Instructor = instructor;

                await _userRepository.CreateAsync(user, cancellationToken);
            }
            else
            {
                // Update existing user
                user = existingUser;
                user.UpdatedAt = DateTimeOffset.UtcNow;
                user.PersonalPictureUrl = googleUserInfo.PictureUrl ?? user.PersonalPictureUrl;

                await _userRepository.UpdateAsync(user, cancellationToken);
            }

            await _userRepository.SaveChangesAsync(cancellationToken);

            // Generate JWT token
            var token = _jwtTokenService.GenerateToken(
                userId: user.Id,
                email: user.GmailExternal ?? string.Empty,
                role: "Instructor",
                fullName: user.FullName
            );

            var tokenExpiration = DateTime.UtcNow.AddMinutes(1440); // 24 hours

            // Return authentication response
            return new AuthenticationResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.GmailExternal ?? string.Empty,
                ProfilePictureUrl = user.PersonalPictureUrl,
                UserRole = "Instructor",
                IsNewUser = isNewUser,
                AuthenticatedAt = DateTimeOffset.UtcNow,
                Token = token,
                TokenExpiresAt = tokenExpiration
            };
        }
    }
}
