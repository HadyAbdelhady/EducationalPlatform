using Domain.Events;
using MediatR;

namespace Application.EventHandlers
{
    public class ExamFinishedEventHandler : INotificationHandler<ExamFinishedEvent>
    {
        public Task Handle(ExamFinishedEvent notification, CancellationToken cancellationToken)
        {
            // Marks are already calculated and saved by SubmitExamCommandHandler.
            // Add side-effects here as needed: push notifications, leaderboard updates, etc.
            return Task.CompletedTask;
        }
    }
}
