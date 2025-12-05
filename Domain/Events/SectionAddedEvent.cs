using MediatR;

namespace Domain.Events
{
    public record SectionAddedEvent(Guid Id,Guid CourseId) : INotification;
}
