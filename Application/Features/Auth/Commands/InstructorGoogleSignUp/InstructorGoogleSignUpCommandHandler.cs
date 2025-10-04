using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auth.Commands.InstructorGoogleSignUp
{
    public class InstructorGoogleSignUpCommandHandler(
        IGoogleAuthService googleAuthService,
        IUserRepository userRepository) : IRequestHandler<InstructorGoogleSignUpCommand, AuthenticationResponse>
    {
        private readonly IGoogleAuthService _googleAuthService = googleAuthService;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<AuthenticationResponse> Handle(InstructorGoogleSignUpCommand request, CancellationToken cancellationToken)
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

            var instructor = new Instructor
            {
                UserId = user.Id
            };

            user.Instructor = instructor;
            
            await _userRepository.CreateAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            return new AuthenticationResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.GmailExternal ?? string.Empty,
                ProfilePictureUrl = user.PersonalPictureUrl,
                UserRole = "Instructor",
                IsNewUser = true,
                AuthenticatedAt = DateTimeOffset.UtcNow
            };
        }
    }
}
