using Domain.Entities;

namespace Application.Interfaces
{
    public interface IStudentEnrollmentRepository
    {
        Task<bool> IsStudentEnrolledInCourseAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);
        Task<bool> IsStudentEnrolledInSectionAsync(Guid studentId, Guid sectionId, CancellationToken cancellationToken = default);
        Task AddStudentCourseAsync(StudentCourse studentCourse, CancellationToken cancellationToken = default);
        Task AddStudentSectionAsync(StudentSection studentSection, CancellationToken cancellationToken = default);
    }
}

