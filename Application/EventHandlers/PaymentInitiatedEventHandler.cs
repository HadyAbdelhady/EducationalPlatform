using Application.Interfaces;
using Domain.Entities;
using Domain.Events;
using Domain.enums;
using MediatR;

namespace Application.EventHandlers
{
    public class PaymentInitiatedEventHandler(IUnitOfWork unitOfWork, IStudentEducationYearProvider studentEducationYearProvider) : INotificationHandler<PaymentInitiatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IStudentEducationYearProvider _studentEducationYearProvider = studentEducationYearProvider;

        public async Task Handle(PaymentInitiatedEvent notification, CancellationToken cancellationToken)
        {
            var paymentData = notification.PaymentData;
            var enrollmentRepo = _unitOfWork.GetRepository<IStudentEnrollmentRepository>();

            var studentEducationYearId = await _studentEducationYearProvider.GetEducationYearIdByUserIdAsync(paymentData.StudentId, cancellationToken);
            if (!studentEducationYearId.HasValue)
            {
                throw new InvalidOperationException("Student not found or has no education year assigned.");
            }

            if (paymentData.EntityType == EntityToBuy.Course)
            {
                var course = await _unitOfWork.Repository<Course>().GetByIdAsync(paymentData.EntityId, cancellationToken)
                    ?? throw new InvalidOperationException("Course not found.");
                if (course.EducationYearId != studentEducationYearId.Value)
                {
                    throw new InvalidOperationException("Cannot enroll in a course from a different education year.");
                }

                // Check if enrollment already exists
                var existingEnrollment = await enrollmentRepo.IsStudentEnrolledInCourseAsync(paymentData.StudentId,
                                                                                             paymentData.EntityId,
                                                                                             cancellationToken);

                if (!existingEnrollment)
                {
                    var studentCourse = new StudentCourse
                    {
                        StudentId = paymentData.StudentId,
                        CourseId = paymentData.EntityId
                    };

                    await enrollmentRepo.AddStudentCourseAsync(studentCourse, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }
            else if (paymentData.EntityType == EntityToBuy.Section)
            {
                var section = await _unitOfWork.Repository<Section>().GetByIdAsync(paymentData.EntityId, cancellationToken, s => s.Course!)
                    ?? throw new InvalidOperationException("Section not found.");
                if (section.Course == null || section.Course.EducationYearId != studentEducationYearId.Value)
                {
                    throw new InvalidOperationException("Cannot enroll in a section from a different education year.");
                }

                // Check if enrollment already exists
                var existingEnrollment = await enrollmentRepo.IsStudentEnrolledInSectionAsync(paymentData.StudentId,
                                                                                              paymentData.EntityId,
                                                                                              cancellationToken);

                if (!existingEnrollment)
                {
                    var studentSection = new StudentSection
                    {
                        StudentId = paymentData.StudentId,
                        SectionId = paymentData.EntityId
                    };

                    await enrollmentRepo.AddStudentSectionAsync(studentSection, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }
}

