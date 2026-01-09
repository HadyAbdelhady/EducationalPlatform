using Application.Interfaces;
using Domain.Entities;
using Domain.Events;
using MediatR;

namespace Application.EventHandlers
{
    public class ExamDeletedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ExamDeletedEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task Handle(ExamDeletedEvent notification, CancellationToken cancellationToken)
        {
            var courseRepo = _unitOfWork.Repository<Course>();

            // Load course + the specific section that owns the exam
            var CoueseList =  courseRepo
                .Find(
                    predicate: c => c.Id == notification.CourseId,
                    cancellationToken: cancellationToken,
                    includes: c => c.Sections.Where(s => s.Id == notification.SectionId)
                );
            if (!CoueseList.Any())
            {
                throw new ArgumentException("No Courses Found");
            }
            var course = CoueseList.FirstOrDefault() ?? throw new ArgumentException($"Could not find course with {notification.CourseId} Found");

            var section = course.Sections.FirstOrDefault() ?? throw new ArgumentException($"Could not find Section with {notification.SectionId} Found");

            // Update domain state
            course.NumberOfExams--;
            section.NumberOfExams--;

            // Apply update (EF Core tracks changes if using change tracking)
            courseRepo.Update(course);

            // Save via UnitOfWork if needed
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }


}
