using Application.DTOs.Auth;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Auth.Commands.InstructorGoogleLogin
{
    /// <summary>
    /// Handles the instructor Google login command.
    /// Validates the Google token, creates or updates the instructor account, and returns authentication response.
    /// </summary>
    public class InstructorGoogleLoginCommandHandler(
        IGoogleAuthService googleAuthService,
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService) : IRequestHandler<InstructorGoogleLoginCommand, Result<AuthenticationResponse>>
    {
        private readonly IGoogleAuthService _googleAuthService = googleAuthService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

        /// <summary>
        /// Handles the instructor Google login process.
        /// </summary>
        /// <param name="request">The instructor Google login command.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Authentication response with user information.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when Google token is invalid.</exception>
        public async Task<Result<AuthenticationResponse>> Handle(InstructorGoogleLoginCommand request, CancellationToken cancellationToken)
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
                    // Create new user and instructor
                    user = new User
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

                    await _unitOfWork.Repository<User>().AddAsync(user, cancellationToken);
                }
                else
                {
                    // Update existing user
                    user = existingUser;
                    user.UpdatedAt = DateTimeOffset.UtcNow;
                    user.PersonalPictureUrl = googleUserInfo.PictureUrl ?? user.PersonalPictureUrl;

                    _unitOfWork.Repository<User>().Update(user);
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
                    UserRole = "Instructor",
                    IsNewUser = isNewUser,
                    AuthenticatedAt = DateTimeOffset.UtcNow,
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
                return Result<AuthenticationResponse>.FailureStatusCode($"Error during Google login: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }
}
