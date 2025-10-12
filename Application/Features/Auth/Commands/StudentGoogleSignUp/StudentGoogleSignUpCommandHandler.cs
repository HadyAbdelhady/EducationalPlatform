using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auth.Commands.StudentGoogleSignUp
{
    public class StudentGoogleSignUpCommandHandler(
        IGoogleAuthService googleAuthService,
        IUserRepository userRepository) : IRequestHandler<StudentGoogleSignUpCommand, AuthenticationResponse>
    {
        private readonly IGoogleAuthService _googleAuthService = googleAuthService;
        private readonly IUserRepository _userRepository = userRepository;
        //private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

        public async Task<AuthenticationResponse> Handle(StudentGoogleSignUpCommand request, CancellationToken cancellationToken)
        {
            var googleUserInfo = await _googleAuthService.ValidateGoogleTokenAsync(request.IdToken, cancellationToken);
            
            if (googleUserInfo == null || !googleUserInfo.EmailVerified)
            {
                throw new UnauthorizedAccessException("Invalid Google token or email not verified.");
            }

            var existingUser = await _userRepository.GetByGoogleEmailAsync(googleUserInfo.Email, cancellationToken);
            
            if (existingUser != null)
            {
                throw new InvalidOperationException("User already exists. Please use login instead.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = googleUserInfo.FullName,
                Ssn = request.Ssn,
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
            await _userRepository.SaveChangesAsync(cancellationToken);

            // Generate JWT token
            //var token = _jwtTokenService.GenerateToken(
            //    userId: user.Id,
            //    email: user.GmailExternal ?? string.Empty,
            //    role: "Student",
            //    fullName: user.FullName
            //);

            //var tokenExpiration = DateTime.UtcNow.AddMinutes(1440); // 24 hours

            return new AuthenticationResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.GmailExternal ?? string.Empty,
                ProfilePictureUrl = user.PersonalPictureUrl,
                UserRole = "Student",
                IsNewUser = true,
                AuthenticatedAt = DateTimeOffset.UtcNow,
                //Token = token,
                //TokenExpiresAt = tokenExpiration
            };
        }
    }
}
