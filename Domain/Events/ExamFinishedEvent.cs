using MediatR;

namespace Domain.Events
{
    public record ExamFinishedEvent(Guid ExamId, Guid StudentId, Guid ExamResultId) : INotification;
}
