using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.AnswersSheets.Commands.DeleteAnswersSheet
{
    public class DeleteAnswersSheetCommandHandler(IUnitOfWork unitOfWork, ICloudinaryCore cloudinaryService)
        : IRequestHandler<DeleteAnswersSheetCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICloudinaryCore _cloudinaryService = cloudinaryService;

        public async Task<Result<string>> Handle(DeleteAnswersSheetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var answersSheet = await _unitOfWork.Repository<AnswersSheet>().GetByIdAsync(
                    request.AnswersSheetId,
                    cancellationToken,
                    x => x.QuestionsSheet);
                if (answersSheet == null)
                {
                    return Result<string>.FailureStatusCode("Answers sheet not found", ErrorType.NotFound);
                }

                if (answersSheet.StudentId != request.StudentId)
                {
                    return Result<string>.FailureStatusCode(
                        "You can only delete your own submission.",
                        ErrorType.UnAuthorized);
                }

                if (answersSheet.IsApproved)
                {
                    return Result<string>.FailureStatusCode(
                        "Cannot delete an approved submission.",
                        ErrorType.Conflict);
                }

                if (answersSheet.QuestionsSheet is not null &&
                    answersSheet.QuestionsSheet.DueDate.HasValue &&
                    DateTimeOffset.UtcNow >= answersSheet.QuestionsSheet.DueDate.Value)
                {
                    return Result<string>.FailureStatusCode(
                        "The submission deadline has passed; this submission can no longer be deleted.",
                        ErrorType.BadRequest);
                }

                var isDeleted = await _cloudinaryService.DeleteSingleMediaAsync(answersSheet.SheetPublicId);
                if (isDeleted)
                {
                    await _unitOfWork.Repository<AnswersSheet>().RemoveAsync(request.AnswersSheetId, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    return Result<string>.Success("Answers sheet deleted successfully");
                }

                return Result<string>.FailureStatusCode($"Error deleting answers sheet with Public Id: {answersSheet.SheetPublicId}", ErrorType.InternalServerError);
            }
            catch (UnauthorizedAccessException auth)
            {
                return Result<string>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<string>.FailureStatusCode($"Error deleting answers sheet: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }
}
