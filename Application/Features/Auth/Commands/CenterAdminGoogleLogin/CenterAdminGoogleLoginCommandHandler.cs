using Application.DTOs.Auth;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Auth.Commands.CenterAdminGoogleLogin
{
    public class CenterAdminGoogleLoginCommandHandler(
        IGoogleAuthService googleAuthService,
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService) : IRequestHandler<CenterAdminGoogleLoginCommand, Result<AuthenticationResponse>>
    {
        private readonly IGoogleAuthService _googleAuthService = googleAuthService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

        public async Task<Result<AuthenticationResponse>> Handle(CenterAdminGoogleLoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var googleUserInfo = await _googleAuthService.ValidateGoogleTokenAsync(request.GoogleUserInfo.IdToken, cancellationToken);

                if (googleUserInfo == false)
                {
                    throw new UnauthorizedAccessException("Invalid Google token or email not verified.");
                }

                // Verify the center exists
                var centerExists = await _unitOfWork.Repository<Center>().AnyAsync(c => c.Id == request.CenterId && !c.IsDeleted, cancellationToken);
                if (!centerExists)
                {
                    return Result<AuthenticationResponse>.FailureStatusCode("The specified center does not exist.", ErrorType.NotFound);
                }

                // Check if user already exists
                var existingUser = await _unitOfWork.GetRepository<IUserRepository>()
                                                         .GetByGoogleEmailAsync(request.GoogleUserInfo.Email, cancellationToken);

                // Reject if the email belongs to a Student or Instructor account without CenterAdmin
                if (existingUser != null && existingUser.CenterAdmin == null)
                {
                    throw new UnauthorizedAccessException("This email is registered with another role. Please use a different email for the Center Admin account.");
                }

                bool isNewUser = existingUser == null;
                User user;

                if (existingUser == null)
                {
                    var ssnExists = await _unitOfWork.Repository<User>().AnyAsync(u => u.Ssn == request.Ssn, cancellationToken);
                    if (ssnExists)
                    {
                        return Result<AuthenticationResponse>.FailureStatusCode("This SSN is already registered with another account.", ErrorType.Conflict);
                    }

                    user = new User
                    {
                        Id = Guid.NewGuid(),
                        FullName = request.GoogleUserInfo.FullName,
                        Ssn = request.Ssn,
                        PhoneNumber = request.GoogleUserInfo.PhoneNumber,
                        GmailExternal = request.GoogleUserInfo.Email,
                        PersonalPictureUrl = request.GoogleUserInfo.PictureUrl,
                        DateOfBirth = request.GoogleUserInfo.DateOfBirth,
                        Gender = request.GoogleUserInfo.Gender,
                        LocationMaps = request.LocationMaps,
                        CreatedAt = EgyptTime.UtcNow,
                        UpdatedAt = EgyptTime.UtcNow,
                        IsDeleted = false
                    };

                    var centerAdmin = new CenterAdmin
                    {
                        UserId = user.Id,
                        CenterId = request.CenterId
                    };

                    user.CenterAdmin = centerAdmin;

                    await _unitOfWork.Repository<User>().AddAsync(user, cancellationToken);
                }
                else
                {
                    user = existingUser;

                    if (user.CenterAdmin != null && user.CenterAdmin.CenterId != request.CenterId)
                    {
                        throw new UnauthorizedAccessException(
                            "This account is already registered as an admin for a different center.");
                    }
                }

                // Generate JWT token with "CenterAdmin" role
                var accesstoken = _jwtTokenService.GenerateToken(
                    userId: user.Id,
                    email: user.GmailExternal ?? string.Empty,
                    role: "CenterAdmin",
                    fullName: user.FullName
                );

                var tokenExpiration = DateTime.UtcNow.AddMinutes(15);

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
                    IsNewUser = isNewUser,
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
