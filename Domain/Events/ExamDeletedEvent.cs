using MediatR;

namespace Domain.Events
{
    public record ExamDeletedEvent(Guid CourseId, Guid SectionId) : INotification;
}
