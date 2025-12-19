using Application.DTOs.Sheets;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Sheets.Commands.UpdateSheet
{
    public class UpdateSheetCommandHandler(IUnitOfWork unitOfWork, ICloudinaryCore cloudinaryService) : IRequestHandler<UpdateSheetCommand, Result<SheetUpdateResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICloudinaryCore _cloudinaryService = cloudinaryService;

        public async Task<Result<SheetUpdateResponse>> Handle(UpdateSheetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var sheet = await _unitOfWork.Repository<Sheet>().GetByIdAsync(request.SheetId, cancellationToken);
                if(sheet is null)
                {
                    return Result<SheetUpdateResponse>.FailureStatusCode("Sheet not found", ErrorType.NotFound);
                }

                if(request.SheetUrl is not null)
                {
                    var cloudinaryResult = await _cloudinaryService.UpdatePdfAsync(sheet.SheetPublicId, request.SheetUrl);
                    sheet.SheetUrl = cloudinaryResult.Url;
                    sheet.SheetPublicId = cloudinaryResult.PublicId;
                }
              
                sheet.DueDate = request.DueDate;
                sheet.Name = request.Name;
                sheet.UpdatedAt = DateTimeOffset.UtcNow;

                _unitOfWork.Repository<Sheet>().Update(sheet);
                await _unitOfWork.Repository<Sheet>().SaveChangesAsync(cancellationToken);

                return Result<SheetUpdateResponse>.Success(new SheetUpdateResponse
                {
                    SheetId = sheet.Id,
                    SheetUrl = sheet.SheetUrl,
                    UpdatedAt = DateTimeOffset.UtcNow
                });
            }
            catch (UnauthorizedAccessException authEx)
            {
                return Result<SheetUpdateResponse>.FailureStatusCode(authEx.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<SheetUpdateResponse>.FailureStatusCode($"Error updating sheet: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }
}
