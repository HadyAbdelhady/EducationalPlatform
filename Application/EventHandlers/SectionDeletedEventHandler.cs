using Application.Interfaces;
using Domain.Events;
using MediatR;

namespace Application.EventHandlers
{
    public class SectionDeletedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<SectionDeletedEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task Handle(SectionDeletedEvent notification, CancellationToken cancellationToken)
        {
            var courseRepo = _unitOfWork.Repository<Domain.Entities.Course>();
            var course = await courseRepo.GetByIdAsync(notification.CourseId, cancellationToken);
            if (course != null)
            {
                course.NumberOfSections -= notification.NumberOfSections;
                courseRepo.Update(course);
            }
        }
    }
}
