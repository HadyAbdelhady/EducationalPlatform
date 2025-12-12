using Application.DTOs.Auth;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Auth.Commands.StudentGoogleLogin
{
    public class StudentGoogleLoginCommandHandler(
        IGoogleAuthService googleAuthService,
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService) : IRequestHandler<StudentGoogleLoginCommand, Result<AuthenticationResponse>>
    {
        private readonly IGoogleAuthService _googleAuthService = googleAuthService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

        public async Task<Result<AuthenticationResponse>> Handle(StudentGoogleLoginCommand request, CancellationToken cancellationToken)
        {
            try
            {


                // Validate Google ID token
                var googleUserInfo = await _googleAuthService.ValidateGoogleTokenAsync(request.GoogleUserInfo.IdToken, cancellationToken);

                if (googleUserInfo == false)
                {
                    throw new UnauthorizedAccessException("Invalid Google token or email not verified.");
                }

                // Check if user already exists
                var existingUser = await _unitOfWork.GetRepository<IUserRepository>()
                                                         .GetByGoogleEmailAsync(request.GoogleUserInfo.Email, cancellationToken);

                bool isNewUser = existingUser == null;
                User user;

                if (existingUser == null)
                {
                    // Create new user and student
                    user = new User
                    {
                        Id = Guid.NewGuid(),
                        FullName = request.GoogleUserInfo.FullName,
                        Ssn = request.Ssn, // Will be updated later if needed
                        PhoneNumber = request.GoogleUserInfo.PhoneNumber,
                        GmailExternal = request.GoogleUserInfo.Email,
                        PersonalPictureUrl = request.GoogleUserInfo.PictureUrl,
                        DateOfBirth = request.GoogleUserInfo.DateOfBirth,
                        Gender = request.GoogleUserInfo.Gender,
                        //EducationYear = request.EducationYear,
                        LocationMaps = request.LocationMaps,
                        CreatedAt = DateTimeOffset.UtcNow,
                        UpdatedAt = DateTimeOffset.UtcNow,

                        IsDeleted = false
                    };

                    var student = new Student
                    {
                        UserId = user.Id,
                        DeviceId = request.DeviceId,
                        ParentPhoneNumber = request.ParentPhoneNumber,

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

                        user.FullName = request.GoogleUserInfo.FullName;
                        user.Ssn = request.Ssn; // Will be updated later if needed
                        user.PhoneNumber = request.GoogleUserInfo.PhoneNumber;
                        user.GmailExternal = request.GoogleUserInfo.Email;
                        user.PersonalPictureUrl = request.GoogleUserInfo.PictureUrl;
                        user.DateOfBirth = request.GoogleUserInfo.DateOfBirth;
                        user.Gender = request.GoogleUserInfo.Gender;
                        //user.Student.EducationYear = request.EducationYear;
                        user.LocationMaps = request.LocationMaps;
                        user.CreatedAt = DateTimeOffset.UtcNow;
                        user.UpdatedAt = DateTimeOffset.UtcNow;
                        user.IsDeleted = false;

                        await _unitOfWork.Repository<User>().AddAsync(user, cancellationToken);
                    }
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
                    //UserRole = "Student",
                    IsNewUser = isNewUser,
                    //AuthenticatedAt = DateTimeOffset.UtcNow,
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
