using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Events;
using MediatR;

namespace Application.Features.Exams.EventHandlers
{
    public class ExamAddedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ExamAddedEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task Handle(ExamAddedEvent notification, CancellationToken cancellationToken)
        {
            var courseRepo = _unitOfWork.Repository<Course>();

            var course = notification.SectionId.HasValue
                ? await courseRepo.FirstOrDefaultAsync(
                    c => c.Id == notification.CourseId,
                    cancellationToken,
                    c => c.Sections.Where(s => s.Id == notification.SectionId))
                : await courseRepo.GetByIdAsync(notification.CourseId, cancellationToken);

            if (course == null)
            {
                throw new ArgumentException($"Could not find course with {notification.CourseId}");
            }

            course.NumberOfExams++;

            var section = notification.SectionId.HasValue
                ? course.Sections.FirstOrDefault(s => s.Id == notification.SectionId)
                : course.Sections.FirstOrDefault();
            if (section != null)
            {
                section.NumberOfExams++;
            }

            courseRepo.Update(course);
        }
    }
}
