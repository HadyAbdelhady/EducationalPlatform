using MediatR;

namespace Domain.Events
{
    public record VideoAddedEvent(Guid Id, int NumberOfVideos) : INotification;
}
