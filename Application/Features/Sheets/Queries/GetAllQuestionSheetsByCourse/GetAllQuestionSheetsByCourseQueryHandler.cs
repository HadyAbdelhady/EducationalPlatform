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

namespace Application.Features.Sheets.Queries.GetAllQuestionSheetsByCourse
{
    public class GetAllQuestionSheetsByCourseQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllQuestionSheetsByCourseQuery, Result<PaginatedResult<SheetResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<PaginatedResult<SheetResponse>>> Handle(GetAllQuestionSheetsByCourseQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var courseExists = await _unitOfWork.Repository<Course>().AnyAsync(x => x.Id == request.CourseId, cancellationToken);
                if (!courseExists)
                    return Result<PaginatedResult<SheetResponse>>.FailureStatusCode("Course not found", ErrorType.NotFound);

                var questionSheets = await _unitOfWork.GetRepository<ISheetRepository>().GetAllSheetsByCourseAsync(request.CourseId, SheetType.QuestionSheet, cancellationToken);

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



