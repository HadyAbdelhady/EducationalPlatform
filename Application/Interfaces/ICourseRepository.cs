using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<IEnumerable<Course>> GetAllCoursesByInstructorIdAsync(Guid instructorId, CancellationToken cancellationToken = default);
    }
}
