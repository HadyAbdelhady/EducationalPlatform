using Application.Common;
using Application.Features.Sheets.DTOs;
using Domain.enums;
using MediatR;

namespace Application.Features.Sheets.Queries.GetSubmittedAnswers
{
    public class GetSubmittedAnswersQuery : IRequest<Result<PaginatedResult<SubmittedAnswersSheetDto>>>
    {
        public required Guid InstructorId { get; init; }
        public required SheetTargetType TargetType { get; init; }
        public required Guid TargetId { get; init; }
        public GetAllEntityRequestSkeleton RequestSkeleton { get; set; } = new GetAllEntityRequestSkeleton();
    }
}
