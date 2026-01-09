using Application.DTOs.Courses;
using Application.Features.Courses.Query.GetAllCourses;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<CourseDetailResponse?> GetCourseDetailResponseByIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    }
}
