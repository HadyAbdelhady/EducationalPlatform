using Domain.Entities;

namespace Application.Features.HomeScreen.Interfaces
{
    public interface IStudentEnrollmentRepository
    {
        Task<bool> IsStudentEnrolledInCourseAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);
        Task<bool> IsStudentEnrolledInSectionAsync(Guid studentId, Guid sectionId, CancellationToken cancellationToken = default);
        Task<bool> CanStudentAccessSectionContentAsync(Guid studentId, Guid sectionId, CancellationToken cancellationToken = default);
        Task AddStudentCourseAsync(StudentCourse studentCourse, CancellationToken cancellationToken = default);
        Task AddStudentSectionAsync(StudentSection studentSection, CancellationToken cancellationToken = default);
        Task EnrollFromPaymentAsync(Guid studentId, Guid? courseId, Guid? sectionId, CancellationToken cancellationToken = default);
        Task<decimal> GetRemainingCoursePriceAsync(Guid studentId, Guid courseId, decimal catalogPrice, CancellationToken cancellationToken = default);
    }
}

