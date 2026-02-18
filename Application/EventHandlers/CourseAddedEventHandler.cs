using Application.Interfaces;
using Domain.Entities;
using Domain.Events;
using MediatR;

namespace Application.EventHandlers
{
    public class CourseAddedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<CourseAddedEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task Handle(CourseAddedEvent notification, CancellationToken cancellationToken)
        {
            // CourseAddedEvent is typically used for side effects like:
            // - Sending notifications to instructors
            // - Initializing default settings
            // - Logging course creation
            
            // For now, we'll keep this handler minimal as the course
            // is already created in the CreateCourseCommandHandler
            // This handler can be extended later for additional side effects
            
            // Example: Could send notification, initialize default settings, etc.
            // var courseRepo = _unitOfWork.Repository<Course>();
            // var course = await courseRepo.GetByIdAsync(notification.Id, cancellationToken);
            // if (course != null) { ... }
        }
    }
}

