using MediatR;

namespace Domain.Events
{
    public record VideoWatchedEvent(Guid VideoId, Guid StudentId) : INotification;
}
