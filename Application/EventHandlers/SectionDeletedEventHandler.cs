using Application.Interfaces;
using Domain.Entities;
using Domain.Events;
using MediatR;

namespace Application.EventHandlers
{
    public class SectionDeletedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<SectionDeletedEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task Handle(SectionDeletedEvent notification, CancellationToken cancellationToken)
        {
            var courseRepo = _unitOfWork.Repository<Course>();
            var course = await courseRepo.GetByIdAsync(notification.CourseId, cancellationToken);
            if (course == null)
            {
                return;
            }
            if (course.NumberOfSections > notification.NumberOfSections)
            {
                course.NumberOfSections -= notification.NumberOfSections;
                courseRepo.Update(course);
            }
            else
            {
                course.NumberOfSections = 0;

            }
        }
    }
}
