using Application.Common;
using Application.Features.Exams.DTOs;
using MediatR;

namespace Application.Features.Exams.Query.GetAllExams
{
    public class GetAllExamsQuery : IRequest<Result<PaginatedResult<ExamListDto>>>
    {
        public GetAllEntityRequestSkeleton RequestSkeleton { get; set; } = new GetAllEntityRequestSkeleton();
        public Guid UserId { get; set; }
    }
}