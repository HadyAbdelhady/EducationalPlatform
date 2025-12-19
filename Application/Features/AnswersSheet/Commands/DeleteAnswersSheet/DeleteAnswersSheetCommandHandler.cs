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
                var answersSheet = await _unitOfWork.Repository<AnswersSheet>().GetByIdAsync(request.AnswersSheetId, cancellationToken);
                if (answersSheet == null)
                {
                    return Result<string>.FailureStatusCode("Answers sheet not found", ErrorType.NotFound);
                }

                var isDeleted = await _cloudinaryService.DeleteSingleMediaAsync(answersSheet.SheetPublicId);
                if (isDeleted)
                {
                    await _unitOfWork.Repository<AnswersSheet>().RemoveAsync(request.AnswersSheetId, cancellationToken);
                    await _unitOfWork.Repository<AnswersSheet>().SaveChangesAsync(cancellationToken);

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


