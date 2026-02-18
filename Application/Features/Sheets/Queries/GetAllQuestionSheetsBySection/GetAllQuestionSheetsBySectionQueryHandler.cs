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

namespace Application.Features.Sheets.Queries.GetAllQuestionSheetsBySection
{
    public class GetAllQuestionSheetsBySectionQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllQuestionSheetsBySectionQuery, Result<PaginatedResult<SheetResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<PaginatedResult<SheetResponse>>> Handle(GetAllQuestionSheetsBySectionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var sectionExists = await _unitOfWork.Repository<Section>().AnyAsync(x => x.Id == request.SectionId, cancellationToken);
                if (!sectionExists)
                    return Result<PaginatedResult<SheetResponse>>.FailureStatusCode("Section not found", ErrorType.NotFound);

                var questionSheets = await _unitOfWork.GetRepository<ISheetRepository>().GetAllSheetsBySectionAsync(request.SectionId, SheetType.QuestionSheet, cancellationToken);

                return Result<PaginatedResult<SheetResponse>>.Success(new PaginatedResult<SheetResponse>
                {
                    Items = questionSheets.ToList(),
                    PageNumber = 1,
                    PageSize = questionSheets.Count,
                    TotalCount = questionSheets.Count
                });
            }
            catch (UnauthorizedAccessException auth)
            {
                return Result<PaginatedResult<SheetResponse>>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<SheetResponse>>.FailureStatusCode(
                    $"An error occurred while retrieving question sheets: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}



