using Application.DTOs.Sheet;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.AnswersSheet.Commands.UpdateAnswersSheet
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
                var answersSheet = await _unitOfWork.Repository<Domain.Entities.AnswersSheet>().GetByIdAsync(request.AnswersSheetId, cancellationToken, x => x.QuestionsSheet);
                if (answersSheet == null)
                {
                    return Result<AnswersSheetUpdateResponse>.FailureStatusCode("Answers sheet not found", ErrorType.NotFound);
                }

                if (DateTime.Now >= answersSheet.QuestionsSheet.DueDate)
                {
                    return Result<AnswersSheetUpdateResponse>.FailureStatusCode("The submission deadline has passed!",
                        ErrorType.BadRequest);
                }

                // Update PDF in Cloudinary
                var cloudinaryResult = await _cloudinaryService.UpdatePdfAsync(answersSheet.SheetPublicId, request.SheetFile);

                answersSheet.SheetUrl = cloudinaryResult.Url;
                answersSheet.SheetPublicId = cloudinaryResult.PublicId;
                answersSheet.Name = request.Name;
                answersSheet.UpdatedAt = DateTimeOffset.UtcNow;

                _unitOfWork.Repository<Domain.Entities.AnswersSheet>().Update(answersSheet);
                await _unitOfWork.Repository<Domain.Entities.AnswersSheet>().SaveChangesAsync(cancellationToken);

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


