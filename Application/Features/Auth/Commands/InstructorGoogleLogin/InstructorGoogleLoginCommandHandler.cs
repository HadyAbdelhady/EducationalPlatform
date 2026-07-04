using Application.Features.Auth.DTOs;
using Application.Common.Interfaces;
using Application.Features.Auth.Interfaces;
using Application.Common;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Auth.Commands.InstructorGoogleLogin
{
    public class InstructorGoogleLoginCommandHandler(
        IGoogleAuthService googleAuthService,
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService) : IRequestHandler<InstructorGoogleLoginCommand, Result<AuthenticationResponse>>
    {
        private readonly IGoogleAuthService _googleAuthService = googleAuthService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

        public async Task<Result<AuthenticationResponse>> Handle(InstructorGoogleLoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate Google ID token
                var isValidToken = await _googleAuthService.ValidateGoogleTokenAsync(request.IdToken, cancellationToken);
                if (isValidToken != true)
                {
                    throw new UnauthorizedAccessException("Invalid Google token or email not verified.");
                }

                // Check if user already exists
                var existingUser = await _unitOfWork.GetRepository<IUserRepository>()
                                                         .GetByGoogleEmailAsync(request.GoogleUserInfo.Email, cancellationToken);

                // Reject if the email belongs to a Student account
                if (existingUser != null && existingUser.Instructor == null)
                {
                    throw new UnauthorizedAccessException("This email is registered as a Student account.");
                }

                bool isNewUser = existingUser == null;
                User user;

                if (existingUser == null)
                {
                    // Check if a user with this SSN already exists
                    var ssnExists = await _unitOfWork.Repository<User>().AnyAsync(u => u.Ssn == request.Ssn, cancellationToken);
                    if (ssnExists)
                    {
                        return Result<AuthenticationResponse>.FailureStatusCode("This SSN is already registered with another account.", ErrorType.Conflict);
                    }

                    // Create new user and instructor
                    user = new User
                    {
                        Id = Guid.NewGuid(),
                        FullName = request.GoogleUserInfo.FullName,
                        Ssn = request.Ssn,
                        PhoneNumber = request.PhoneNumber,
                        GmailExternal = request.GoogleUserInfo.Email,
                        PersonalPictureUrl = request.GoogleUserInfo.PictureUrl,
                        Gender = request.Gender,
                        CreatedAt = EgyptTime.UtcNow,
                        UpdatedAt = EgyptTime.UtcNow,
                        IsDeleted = false
                    };

                    // Step 1: Create instructor without PreferencesId first (breaks circular FK)
                    var instructor = new Instructor
                    {
                        UserId = user.Id,
                        PreferencesId = null
                    };

                    user.Instructor = instructor;
                    await _unitOfWork.Repository<User>().AddAsync(user, cancellationToken);
                    // Save user + instructor first so InstructorPreferences can reference instructor_id
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    // Step 2: Create InstructorPreferences now that instructor exists
                    var instructorPreferences = new InstructorPreferences
                    {
                        Id = Guid.NewGuid(),
                        InstructorId = user.Id,
                        ApplicationName = string.IsNullOrWhiteSpace(request.ApplicationName) ? user.FullName : request.ApplicationName,
                        CreatedAt = EgyptTime.UtcNow,
                        IsDeleted = false
                    };
                    await _unitOfWork.Repository<InstructorPreferences>().AddAsync(instructorPreferences, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    // Step 3: Link PreferencesId back onto instructor
                    instructor.PreferencesId = instructorPreferences.Id;
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    user = existingUser;
                }

                // Generate JWT token
                var token = _jwtTokenService.GenerateToken(
                    userId: user.Id,
                    email: user.GmailExternal ?? string.Empty,
                    role: "Instructor",
                    fullName: user.FullName
                );

                var tokenExpiration = DateTime.UtcNow.AddMinutes(1440); // 24 hours

                // Generate refresh token
                var refreshToken = _jwtTokenService.GenerateRefreshToken();
                await _unitOfWork.GetRepository<IRefreshTokenRepository>().AddRefreshTokenAsync(refreshToken, user.Id, cancellationToken);

                // Save all changes in a single transaction
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<AuthenticationResponse>.Success(new AuthenticationResponse
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.GmailExternal ?? string.Empty,
                    ProfilePictureUrl = user.PersonalPictureUrl,
                    IsNewUser = isNewUser,
                    Token = token,
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
                var innerMsg = ex.InnerException != null ? $" Inner: {ex.InnerException.Message}" : "";
                return Result<AuthenticationResponse>.FailureStatusCode($"Error during Google login: {ex.Message}{innerMsg}", ErrorType.InternalServerError);
            }
        }
    }
}
