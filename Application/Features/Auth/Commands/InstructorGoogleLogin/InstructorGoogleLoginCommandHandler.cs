using Application.DTOs.Auth;
using Application.Interfaces;
using Application.ResultWrapper;
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
                var googleUserInfo = await _googleAuthService.ValidateGoogleTokenAsync(request.IdToken, cancellationToken);


                // Check if user already exists
                var existingUser = await _unitOfWork.GetRepository<IUserRepository>()
                                                         .GetByGoogleEmailAsync(request.GoogleUserInfo.Email, cancellationToken);

                bool isNewUser = existingUser == null;
                User user;

                if (existingUser == null)
                {
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

                // Generate JWT token
                var token = _jwtTokenService.GenerateToken(
                    userId: existingUser!.Id,
                    email: existingUser.GmailExternal ?? string.Empty,
                    role: "Instructor",
                    fullName: existingUser.FullName
                );

                var tokenExpiration = DateTime.UtcNow.AddMinutes(1440); // 24 hours

                // Generate refresh token
                var refreshToken = _jwtTokenService.GenerateRefreshToken();
                await _unitOfWork.GetRepository<IRefreshTokenRepository>().AddRefreshTokenAsync(refreshToken, existingUser.Id, cancellationToken);

                // Save all changes in a single transaction
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<AuthenticationResponse>.Success(new AuthenticationResponse
                {
                    UserId = existingUser.Id,
                    FullName = existingUser.FullName,
                    Email = existingUser.GmailExternal ?? string.Empty,
                    ProfilePictureUrl = existingUser.PersonalPictureUrl,
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
                return Result<AuthenticationResponse>.FailureStatusCode($"Error during Google login: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }
}
