using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Exams.Command.StartExam
{
    public class StartExamCommand : IRequest<Result<StartedExamResponse>>
    {
        public Guid Student { get; set; }
        public Guid ExamId { get; set; }

    }

    public class StartedExamResponse
    {
        public Guid Student { get; set; }
        public Guid ExamId { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.Now;
    }
}
