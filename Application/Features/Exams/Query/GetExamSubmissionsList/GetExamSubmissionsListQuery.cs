using Application.DTOs;
using Application.DTOs.Exam;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Exams.Query.GetExamSubmissionsList
{
    public class GetExamSubmissionsListQuery : IRequest<Result<PaginatedResult<ExamSubmissionDto>>>
    {
        public Guid ExamId { get; set; }
        public GetAllEntityRequestSkeleton RequestSkeleton { get; set; } = new GetAllEntityRequestSkeleton();
        public Guid UserId { get; set; }
    }
}
