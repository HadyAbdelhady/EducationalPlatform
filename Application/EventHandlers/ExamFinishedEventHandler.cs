using Application.Interfaces;
using Domain.Entities;
using Domain.Events;
using MediatR;

namespace Application.EventHandlers
{
    public class ExamFinishedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ExamFinishedEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task Handle(ExamFinishedEvent notification, CancellationToken cancellationToken)
        {
            // ExamFinishedEvent is typically used for side effects like:
            // - Sending notifications
            // - Updating statistics
            // - Triggering follow-up actions
            
            // For now, we'll keep this handler minimal as the exam result
            // is already saved in the SubmitExamCommandHandler
            // This handler can be extended later for additional side effects
            
            // Example: Could update exam statistics, send notifications, etc.
            // var examRepo = _unitOfWork.Repository<Exam>();
            // var exam = await examRepo.GetByIdAsync(notification.ExamId, cancellationToken);
            // if (exam != null) { ... }
        }
    }
}

