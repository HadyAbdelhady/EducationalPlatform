using MediatR;

namespace Domain.Events
{
    public record VideoDeletedEvent(Guid Id, Guid SectionId, Guid CourseId) : INotification;
}
