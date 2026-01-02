using Application.Interfaces;
using Domain.Entities;
using Domain.Events;
using MediatR;

namespace Application.EventHandlers
{
    public class CourseDeletedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<CourseDeletedEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task Handle(CourseDeletedEvent notification, CancellationToken cancellationToken)
        {
            // CourseDeletedEvent is typically used for cleanup and side effects like:
            // - Sending notifications to enrolled students
            // - Cleaning up related data
            // - Logging course deletion
            
            // Note: The course is already soft-deleted in DeleteCourseCommandHandler
            // This handler can be used for additional cleanup or notifications
            
            // Example: Could send notifications, cleanup related entities, etc.
            // var courseRepo = _unitOfWork.Repository<Course>();
            // var course = await courseRepo.GetByIdAsync(notification.CourseId, cancellationToken);
            // if (course != null) { ... }
        }
    }
}

