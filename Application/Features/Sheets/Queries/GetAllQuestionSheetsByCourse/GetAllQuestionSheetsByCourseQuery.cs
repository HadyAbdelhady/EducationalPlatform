using Application.DTOs.Sheets;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Sheets.Queries.GetAllQuestionSheetsByCourse
{
    public class GetAllQuestionSheetsByCourseQuery : IRequest<Result<PaginatedResult<SheetResponse>>>
    {
        public Guid CourseId { get; set; }
    }
}



