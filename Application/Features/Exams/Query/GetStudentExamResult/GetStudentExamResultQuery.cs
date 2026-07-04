using Application.Features.Exams.DTOs;
using Application.Common;
using MediatR;

namespace Application.Features.Exams.Query.GetStudentExamResult
{
    public class GetStudentExamResultQuery : IRequest<Result<ExamSubmissionDto>>
    {
        public Guid ExamId { get; set; }
        public Guid StudentId { get; set; }
    }
}
