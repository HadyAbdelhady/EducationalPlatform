using MediatR;

namespace Domain.Events
{
    public record CourseAddedEvent(Guid Id) : INotification;
}
