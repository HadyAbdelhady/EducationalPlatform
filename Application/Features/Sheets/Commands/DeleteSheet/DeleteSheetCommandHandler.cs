using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Sheets.Commands.DeleteSheet
{
    public class DeleteSheetCommandHandler(IUnitOfWork unitOfWork, ICloudinaryCore cloudinaryService) : IRequestHandler<DeleteSheetCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICloudinaryCore _cloudinaryService = cloudinaryService;

        public async Task<Result<string>> Handle(DeleteSheetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var sheet = await _unitOfWork.Repository<Sheet>().GetByIdAsync(request.SheetId, cancellationToken);
                if (sheet is null)
                {
                    return Result<string>.FailureStatusCode("Sheet not found", ErrorType.NotFound);
                }

                var isDeleted = await _cloudinaryService.DeleteSingleMediaAsync(sheet.SheetPublicId);
                if (isDeleted)
                {
                    await _unitOfWork.Repository<Sheet>().RemoveAsync(request.SheetId, cancellationToken);
                    await _unitOfWork.Repository<Sheet>().SaveChangesAsync(cancellationToken);

                    return Result<string>.Success("Sheet deleted successfully");
                }

                return Result<string>.FailureStatusCode($"Error deleting sheet with Public Id: {sheet.SheetPublicId}", ErrorType.InternalServerError);
            }
            catch (UnauthorizedAccessException auth)
            {
                return Result<string>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<string>.FailureStatusCode($"Error deleting sheet: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }
}
