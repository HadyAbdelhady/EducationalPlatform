using Application.Interfaces;
using Domain.Entities;
using Domain.Events;
using MediatR;

namespace Application.EventHandlers
{
    public class StudentEnrolledEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<StudentEnrolledEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task Handle(StudentEnrolledEvent notification, CancellationToken cancellationToken)
        {
            // Check if enrollment is for a course (StudentCourse) or section (StudentSection)
            // We'll check both to determine which entity was enrolled in
            var courseRepo = _unitOfWork.Repository<Course>();
            var course = await courseRepo.GetByIdAsync(notification.EnrollmentEntityId, cancellationToken);
            
            if (course != null)
            {
                // Enrollment is for a course
                course.NumberOfStudentsEnrolled++;
                courseRepo.Update(course);
            }
            else
            {
                // Check if it's a section enrollment - sections don't have a direct enrollment counter
                // but we might want to track this in the future
                // For now, we'll just handle course enrollments
            }
        }
    }
}

