using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Exams.Command.StartExam
{
    public class StartExamCommand : IRequest<Result<StartedExamResponse>>
    {
        public Guid Student { get; set; }
        public Guid ExamId { get; set; }
        public bool IsRandomized { get; set; } = false;

    }

    public class StartedExamResponse
    {
        public Guid Student { get; set; }
        public Guid ExamId { get; set; }
        public DateTimeOffset StartedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset CurrentTime { get; set; } = DateTimeOffset.UtcNow;
    }
}
