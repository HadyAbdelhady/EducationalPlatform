using Application.DTOs.Sheets;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Sheets.Queries.GetAllTutorialSheetsByCourse
{
    public class GetAllTutorialSheetsByCourseQuery : IRequest<Result<PaginatedResult<SheetResponse>>>
    {
        public Guid CourseId { get; set; }
    }
}
