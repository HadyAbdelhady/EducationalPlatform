using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Events;
using MediatR;

namespace Application.Features.Exams.EventHandlers
{
    public class ExamDeletedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ExamDeletedEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task Handle(ExamDeletedEvent notification, CancellationToken cancellationToken)
        {
            var courseRepo = _unitOfWork.Repository<Course>();

            var course = await courseRepo.FirstOrDefaultAsync(
                c => c.Id == notification.CourseId,
                cancellationToken,
                c => c.Sections.Where(s => s.Id == notification.SectionId));

            if (course == null)
            {
                throw new ArgumentException($"Could not find course with {notification.CourseId}");
            }

            var section = course.Sections.FirstOrDefault(s => s.Id == notification.SectionId)
                ?? throw new ArgumentException($"Could not find Section with {notification.SectionId}");

            course.NumberOfExams--;
            section.NumberOfExams--;

            courseRepo.Update(course);
        }
    }
}
