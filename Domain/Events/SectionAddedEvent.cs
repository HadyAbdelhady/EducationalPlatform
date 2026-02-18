using MediatR;

namespace Domain.Events
{
    public record SectionAddedEvent(Guid CourseId, int NumberOfSections) : INotification;
}
