using Application.Common;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Profiles.Commands.UpdateStudentProfilePicture
{
    public class UpdateStudentProfilePictureCommandHandler(
        IUnitOfWork unitOfWork,
        ICloudinaryCore cloudinaryService)
        : IRequestHandler<UpdateStudentProfilePictureCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICloudinaryCore _cloudinaryService = cloudinaryService;

        public async Task<Result<string>> Handle(
            UpdateStudentProfilePictureCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (request.PictureFile is null || request.PictureFile.Length == 0)
                {
                    return Result<string>.FailureStatusCode(
                        "A profile picture file is required.",
                        ErrorType.BadRequest);
                }

                var user = await _unitOfWork.Repository<User>()
                    .GetByIdAsync(request.StudentId, cancellationToken, u => u.Student!);
                if (user?.Student is null)
                {
                    return Result<string>.FailureStatusCode("Student not found.", ErrorType.Forbidden);
                }

                var pictureUrl = await _cloudinaryService.UploadMediaAsync(
                    request.PictureFile,
                    UsageCategory.ProfilePicture);

                user.PersonalPictureUrl = pictureUrl;
                user.UpdatedAt = EgyptTime.UtcNow;
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<string>.Success(pictureUrl);
            }
            catch (ArgumentException ex)
            {
                return Result<string>.FailureStatusCode(ex.Message, ErrorType.BadRequest);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Result<string>.FailureStatusCode(ex.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<string>.FailureStatusCode(
                    $"An error occurred while updating the profile picture: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
