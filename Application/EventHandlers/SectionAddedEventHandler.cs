using Application.Interfaces;
using Domain.Events;
using MediatR;

namespace Application.EventHandlers
{
    public class SectionAddedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<SectionAddedEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task Handle(SectionAddedEvent notification, CancellationToken cancellationToken)
        {
            var courseRepo = _unitOfWork.Repository<Domain.Entities.Course>();
            var course = await courseRepo.GetByIdAsync(notification.CourseId, cancellationToken);
            if (course != null)
            {
                course.NumberOfSections += 1;
                courseRepo.Update(course);
            }
        }
    }
}
