using Application.Common;
using Application.Features.Sheets.DTOs;
using MediatR;

namespace Application.Features.Sheets.Queries.GetInstructorSheetsWithSubmissions
{
    public class GetInstructorSheetsWithSubmissionsQuery : IRequest<Result<PaginatedResult<QuestionSheetWithSubmissionsDto>>>
    {
        public required Guid InstructorId { get; init; }
        public GetAllEntityRequestSkeleton RequestSkeleton { get; set; } = new GetAllEntityRequestSkeleton();
    }
}
