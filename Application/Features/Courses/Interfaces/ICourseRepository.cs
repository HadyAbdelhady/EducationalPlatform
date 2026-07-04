using Application.Common.Interfaces;
using Application.Features.Courses.DTOs;
using Application.Features.Courses.Query.GetCourseById;
using Domain.Entities;

namespace Application.Features.Courses.Interfaces
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<CourseDetailResponse?> GetCourseDetailResponseByIdAsync(GetCourseByIdQuery request, CancellationToken cancellationToken = default);
    }
}
