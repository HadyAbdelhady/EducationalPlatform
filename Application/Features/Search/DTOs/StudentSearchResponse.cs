using Application.Common;

namespace Application.Features.Search.DTOs
{
    public class StudentSearchResponse
    {
        public PaginatedResult<CourseSearchItemDto> Courses { get; set; } = new();

        public PaginatedResult<SectionSearchItemDto> Sections { get; set; } = new();

        public PaginatedResult<InstructorSearchItemDto>? Instructors { get; set; }
    }

    public class CourseSearchItemDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? PictureUrl { get; set; }

        public decimal Price { get; set; }

        public bool IsEnrolled { get; set; }
    }

    public class SectionSearchItemDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public Guid CourseId { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public bool IsEnrolled { get; set; }
    }

    public class InstructorSearchItemDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string? PersonalPictureUrl { get; set; }
    }
}
