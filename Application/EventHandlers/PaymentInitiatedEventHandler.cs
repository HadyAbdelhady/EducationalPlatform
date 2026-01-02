using Application.Interfaces;
using Domain.Entities;
using Domain.Events;
using Domain.enums;
using MediatR;

namespace Application.EventHandlers
{
    public class PaymentInitiatedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<PaymentInitiatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task Handle(PaymentInitiatedEvent notification, CancellationToken cancellationToken)
        {
            var paymentData = notification.PaymentData;
            var enrollmentRepo = _unitOfWork.GetRepository<IStudentEnrollmentRepository>();

            if (paymentData.EntityType == EntityToBuy.Course)
            {
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

