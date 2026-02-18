using Application.DTOs.Courses;
using Application.Features.Courses.Query.GetCourseById;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<CourseDetailResponse?> GetCourseDetailResponseByIdAsync(GetCourseByIdQuery request, CancellationToken cancellationToken = default);
    }
}
