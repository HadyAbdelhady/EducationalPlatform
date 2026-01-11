using Application.DTOs.Courses;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Courses.Query.GetAllCourses
{
    public class GetAllCoursesQuery : IRequest<Result<PaginatedResult<CourseResponse>>>
    {
        public Dictionary<string, string> Filters { get; set; } = [];

        public string SortBy { get; set; } = "createdat";

        public bool IsDescending { get; set; } = false;

        public int PageNumber { get; set; } = 1;
        public Guid UserID { get; set; }

    }

    public class GetAllCoursesRequest
    {
        public Dictionary<string, string> Filters { get; set; } = [];

        public string SortBy { get; set; } = "createdat";

        public bool IsDescending { get; set; } = false;

        public int PageNumber { get; set; } = 1;
    }
}