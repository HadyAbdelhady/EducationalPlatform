using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Courses.DTOs;
using Application.Features.Courses.Query.GetAllCourses;
using Application.Features.Courses.Query.GetCourseById;
using Application.Features.EducationYears.Interfaces;
using Domain.Entities;

namespace Application.Features.Courses.Interfaces
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<CourseDetailResponse?> GetCourseDetailResponseByIdAsync(GetCourseByIdQuery request, CancellationToken cancellationToken = default);
        Task<PaginatedResult<CourseResponse>> GetCoursesPaginatedAsync(
            GetAllCoursesQuery request,
            IBaseFilterRegistry<Course> courseFilterRegistry,
            IStudentEducationYearProvider studentEducationYearProvider,
            CancellationToken cancellationToken = default);
    }
}
