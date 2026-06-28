using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Exams.Command.StartExam
{
    public class StartExamCommand : IRequest<Result<StartedExamResponse>>
    {
        public Guid StudentId { get; set; }
        public Guid ExamId { get; set; }

    }

    public class StartedExamResponse
    {
        public Guid Student { get; set; }
        public Guid ExamId { get; set; }
        public DateTimeOffset StartedAt { get; set; } = EgyptTime.UtcNow;
        public DateTimeOffset CurrentTime { get; set; } = EgyptTime.UtcNow;
    }
}
