using Application.DTOs;
using Application.DTOs.Exam;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Exams.Query.GetStudentsSubmittionsForExam
{
    public class GetStudentsSubmittionsForExamQuery : IRequest<Result<PaginatedResult<ExamListDto>>>
    {
        public Guid ExamId { get; set; }
        public GetAllEntityRequestSkeleton RequestSkeleton { get; set; } = new GetAllEntityRequestSkeleton();
        public Guid UserId { get; set; }
    }
}
