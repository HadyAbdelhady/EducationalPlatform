using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<IEnumerable<Course>> GetAllCoursesByInstructorIdAsync(Guid instructorId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Course>> GetAllCoursesByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<Course?> GetCourseDetailByIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    }
}
