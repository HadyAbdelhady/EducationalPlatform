using Application.Common;
using Application.Features.Exams.DTOs;
using MediatR;

namespace Application.Features.Exams.Query.GetLiveExam
{
    public class GetLiveExamQuery : IRequest<Result<LiveExamResponse>>
    {
        public Guid ExamId { get; set; }
        public Guid InstructorId { get; set; }
    }
}
