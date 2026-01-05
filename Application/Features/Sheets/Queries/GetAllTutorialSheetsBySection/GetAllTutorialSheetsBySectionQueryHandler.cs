using Application.DTOs.Sheets;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Sheets.Queries.GetAllTutorialSheetsBySection
{
    public class GetAllTutorialSheetsBySectionQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllTutorialSheetsBySectionQuery, Result<PaginatedResult<SheetResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<PaginatedResult<SheetResponse>>> Handle(GetAllTutorialSheetsBySectionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var sectionExists = await _unitOfWork.Repository<Section>().AnyAsync(x => x.Id == request.SectionId, cancellationToken);
                if (!sectionExists)
                    return Result<PaginatedResult<SheetResponse>>.FailureStatusCode("Section not found", ErrorType.NotFound);

                var tutorialSheets = await _unitOfWork.GetRepository<ISheetRepository>().GetAllSheetsBySectionAsync(request.SectionId, SheetType.TutorialSheet,cancellationToken);

                return Result<PaginatedResult<SheetResponse>>.Success(new PaginatedResult<SheetResponse>
                {
                    Items = tutorialSheets.ToList(),
                    PageNumber = 1,
                    PageSize = tutorialSheets.Count,
                    TotalCount = tutorialSheets.Count
                });
            }
            catch (UnauthorizedAccessException auth)
            {
                return Result<PaginatedResult<SheetResponse>>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<SheetResponse>>.FailureStatusCode(
                    $"An error occurred while retrieving tutorial sheets: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}



