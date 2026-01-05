using Application.DTOs.Courses;
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

namespace Application.Features.Sheets.Queries.GetAllTutorialSheetsByCourse
{
    public class GetAllTutorialSheetsByCourseQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllTutorialSheetsByCourseQuery, Result<PaginatedResult<SheetResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<PaginatedResult<SheetResponse>>> Handle(GetAllTutorialSheetsByCourseQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var courseExists = await _unitOfWork.Repository<Course>().AnyAsync(x => x.Id == request.CourseId, cancellationToken);
                if (!courseExists)
                    return Result<PaginatedResult<SheetResponse>>.FailureStatusCode("Course not found", ErrorType.NotFound);

                var tutorialSheets = await _unitOfWork.GetRepository<ISheetRepository>().GetAllSheetsByCourseAsync(request.CourseId, SheetType.TutorialSheet, cancellationToken);

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
