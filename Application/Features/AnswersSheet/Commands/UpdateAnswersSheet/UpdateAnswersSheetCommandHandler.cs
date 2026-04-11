using Application.DTOs.Sheets;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.AnswersSheets.Commands.UpdateAnswersSheet
{
    public class UpdateAnswersSheetCommandHandler(IUnitOfWork unitOfWork, ICloudinaryCore cloudinaryService)
        : IRequestHandler<UpdateAnswersSheetCommand, Result<AnswersSheetUpdateResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICloudinaryCore _cloudinaryService = cloudinaryService;

        public async Task<Result<AnswersSheetUpdateResponse>> Handle(UpdateAnswersSheetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return Result<AnswersSheetUpdateResponse>.FailureStatusCode("Name is required.", ErrorType.BadRequest);
                }

                if (request.SheetFile is null || request.SheetFile.Length == 0)
                {
                    return Result<AnswersSheetUpdateResponse>.FailureStatusCode("A non-empty PDF file is required.", ErrorType.BadRequest);
                }

                var contentType = request.SheetFile.ContentType ?? string.Empty;
                var fileName = request.SheetFile.FileName ?? string.Empty;
                if (!contentType.Contains("pdf", StringComparison.OrdinalIgnoreCase) &&
                    !fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    return Result<AnswersSheetUpdateResponse>.FailureStatusCode("Only PDF uploads are allowed.", ErrorType.BadRequest);
                }

                var answersSheet = await _unitOfWork.Repository<AnswersSheet>().GetByIdAsync(request.AnswersSheetId, cancellationToken, x => x.QuestionsSheet);
                if (answersSheet == null)
                {
                    return Result<AnswersSheetUpdateResponse>.FailureStatusCode("Answers sheet not found", ErrorType.NotFound);
                }

                if (answersSheet.StudentId != request.StudentId)
                {
                    return Result<AnswersSheetUpdateResponse>.FailureStatusCode(
                        "You can only update your own submission.",
                        ErrorType.UnAuthorized);
                }

                if (answersSheet.IsApproved)
                {
                    return Result<AnswersSheetUpdateResponse>.FailureStatusCode(
                        "Cannot modify an approved submission.",
                        ErrorType.Conflict);
                }

                if (answersSheet.QuestionsSheet is null)
                {
                    return Result<AnswersSheetUpdateResponse>.FailureStatusCode("Questions sheet not found.", ErrorType.NotFound);
                }

                if (answersSheet.QuestionsSheet.DueDate.HasValue && DateTimeOffset.UtcNow >= answersSheet.QuestionsSheet.DueDate.Value)
                {
                    return Result<AnswersSheetUpdateResponse>.FailureStatusCode("The submission deadline has passed!",
                        ErrorType.BadRequest);
                }

                var cloudinaryResult = await _cloudinaryService.UpdatePdfAsync(answersSheet.SheetPublicId, request.SheetFile);

                answersSheet.SheetUrl = cloudinaryResult.Url;
                answersSheet.SheetPublicId = cloudinaryResult.PublicId;
                answersSheet.Name = request.Name.Trim();
                answersSheet.UpdatedAt = DateTimeOffset.UtcNow;

                _unitOfWork.Repository<AnswersSheet>().Update(answersSheet);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<AnswersSheetUpdateResponse>.Success(new AnswersSheetUpdateResponse
                {
                    AnswersSheetId = answersSheet.Id,
                    SheetUrl = answersSheet.SheetUrl,
                    UpdatedAt = answersSheet.UpdatedAt ?? DateTimeOffset.UtcNow
                });
            }
            catch (UnauthorizedAccessException authEx)
            {
                return Result<AnswersSheetUpdateResponse>.FailureStatusCode(authEx.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<AnswersSheetUpdateResponse>.FailureStatusCode($"Error updating answers sheet: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }
}
