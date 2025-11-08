using Application.DTOs.Auth;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Auth.Commands.StudentGoogleLogin
{
    /// <summary>
    /// Handles the student Google login command.
    /// Validates the Google token, creates or updates the student account, and returns authentication response.
    /// </summary>
    public class StudentGoogleLoginCommandHandler(
        IGoogleAuthService googleAuthService,
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService) : IRequestHandler<StudentGoogleLoginCommand, Result<AuthenticationResponse>>
    {
        private readonly IGoogleAuthService _googleAuthService = googleAuthService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

        /// <summary>
        /// Handles the student Google login process.
        /// </summary>
        /// <param name="request">The student Google login command.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Authentication response with user information.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when Google token is invalid.</exception>
        public async Task<Result<AuthenticationResponse>> Handle(StudentGoogleLoginCommand request, CancellationToken cancellationToken)
        {
            try
            {


                // Validate Google ID token
                var googleUserInfo = await _googleAuthService.ValidateGoogleTokenAsync(request.IdToken, cancellationToken);

                if (googleUserInfo == null || !googleUserInfo.EmailVerified)
                {
                    throw new UnauthorizedAccessException("Invalid Google token or email not verified.");
                }

                // Check if user already exists
                var existingUser = await _unitOfWork.GetRepository<IUserRepository>()
                                                         .GetByGoogleEmailAsync(googleUserInfo.Email, cancellationToken);

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
                        TriedScreenshot = false
                    };

                    user.Student = student;

                    await _unitOfWork.Repository<User>().AddAsync(user, cancellationToken);
                }
                else
                {
                    // Validate device ID for existing student
                    user = existingUser;

                    if (user.Student != null)
                    {
                        // Check if the deviceID matches the stored one
                        if (!string.IsNullOrEmpty(user.Student.DeviceId) &&
                            user.Student.DeviceId != request.DeviceId)
                        {
                            throw new UnauthorizedAccessException(
                                "Login attempt detected from a different device. " +
                                "Please use your registered device to access your account.");
                        }

                        // Update deviceID if it was null/empty (for backward compatibility)
                        if (string.IsNullOrEmpty(user.Student.DeviceId))
                        {
                            user.Student.DeviceId = request.DeviceId;
                        }
                    }

                    user.UpdatedAt = DateTimeOffset.UtcNow;
                    user.PersonalPictureUrl = googleUserInfo.PictureUrl ?? user.PersonalPictureUrl;

                    _unitOfWork.Repository<User>().Update(user);
                }

                // Generate JWT token
                var accesstoken = _jwtTokenService.GenerateToken(
                    userId: user.Id,
                    email: user.GmailExternal ?? string.Empty,
                    role: "Student",
                    fullName: user.FullName
                );

                var tokenExpiration = DateTime.UtcNow.AddMinutes(1440); // 24 hours

                // Generate refresh token
                var refreshToken = _jwtTokenService.GenerateRefreshToken();
                await _unitOfWork.GetRepository<IRefreshTokenRepository>().AddRefreshTokenAsync(refreshToken, user.Id, cancellationToken);

                // Save all changes in a single transaction
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Return authentication response
                return Result<AuthenticationResponse>.Success(new AuthenticationResponse
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.GmailExternal ?? string.Empty,
                    ProfilePictureUrl = user.PersonalPictureUrl,
                    UserRole = "Student",
                    IsNewUser = isNewUser,
                    AuthenticatedAt = DateTimeOffset.UtcNow,
                    Token = accesstoken,
                    TokenExpiresAt = tokenExpiration,
                    RefreshToken = refreshToken
                });
            }
            catch (UnauthorizedAccessException auth)
            {
                return Result<AuthenticationResponse>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<AuthenticationResponse>.FailureStatusCode($"Error during Google login: {ex.Message}", ErrorType.InternalServerError);
            }

        }
    }
}
