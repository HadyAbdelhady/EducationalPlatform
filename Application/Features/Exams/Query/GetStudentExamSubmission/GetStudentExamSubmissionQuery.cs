using Application.Common;
using Application.Features.Exams.DTOs;
using MediatR;

namespace Application.Features.Exams.Query.GetStudentExamSubmission
{
    public class GetStudentExamSubmissionQuery : IRequest<Result<StudentExamSubmissionDetailDto>>
    {
        public Guid ExamId { get; set; }
        public Guid StudentId { get; set; }
        public Guid CallerUserId { get; set; }
        public GetAllEntityRequestSkeleton RequestSkeleton { get; set; } = new();
    }
}
