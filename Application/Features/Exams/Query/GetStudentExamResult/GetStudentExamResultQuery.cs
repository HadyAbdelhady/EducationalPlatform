using Application.DTOs.Exam;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Exams.Query.GetStudentExamResult
{
    public class GetStudentExamResultQuery : IRequest<Result<ExamSubmissionDto>>
    {
        public Guid ExamId { get; set; }
        public Guid StudentId { get; set; }
    }
}
