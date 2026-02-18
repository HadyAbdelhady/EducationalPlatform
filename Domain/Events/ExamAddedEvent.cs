using MediatR;

namespace Domain.Events
{
    public record ExamAddedEvent(Guid CourseId, Guid? SectionId) : INotification;
}
