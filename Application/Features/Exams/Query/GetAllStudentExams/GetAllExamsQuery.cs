using Application.ResultWrapper;
using Application.DTOs.Exam;
using Application.DTOs;
using MediatR;

namespace Application.Features.Exams.Query.GetAllStudentExams
{
    public class GetAllExamsQuery : IRequest<Result<PaginatedResult<ExamListDto>>>
    {
        public GetAllEntityRequestSkeleton RequestSkeleton { get; set; } = new GetAllEntityRequestSkeleton();
        public Guid UserId { get; set; }
    }
}