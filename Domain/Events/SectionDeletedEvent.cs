using MediatR;

namespace Domain.Events
{
    public record SectionDeletedEvent(Guid CourseId, int NumberOfSections) : INotification;
}
