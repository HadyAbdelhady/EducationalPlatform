using Application.DTOs.Auth;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Auth.Queries.CheckUserExists
{
    public class CheckUserExistsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<CheckUserExistsQuery, Result<CheckUserExistsResponse?>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<CheckUserExistsResponse?>> Handle(CheckUserExistsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userRepo = _unitOfWork.GetRepository<IUserRepository>();
                var user = await userRepo.GetByGoogleEmailAsync(request.Email, cancellationToken);

                if (user == null)
                {
                    // User does not exist, return null
                    return Result<CheckUserExistsResponse?>.FailureStatusCode("User does not exist", ErrorType.NotFound);
                }

                string role = "User";
                if (user.CenterAdmin != null)
                {
                    role = "CenterAdmin";
                }
                else if (user.Instructor != null)
                {
                    role = "Instructor";
                }
                else if (user.Student != null)
                {
                    role = "Student";
                }

                var response = new CheckUserExistsResponse
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.GmailExternal ?? request.Email,
                    ProfilePictureUrl = user.PersonalPictureUrl,
                    Role = role
                };

                return Result<CheckUserExistsResponse?>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<CheckUserExistsResponse?>.FailureStatusCode($"An error occurred while checking if user exists: {ex.Message}", Domain.enums.ErrorType.InternalServerError);
            }
        }
    }
}
