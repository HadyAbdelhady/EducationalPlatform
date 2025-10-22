using Application.DTOs.Course;
using MediatR;

namespace Application.Features.Course.Commands.CreateCourse
{
    public record CreateCourseCommand : IRequest<CourseCreationResponse>
    {
        public string CourseName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid InstructorId { get; set; }
        public decimal? Price { get; set; }
        public string? PictureUrl { get; set; }
        public string? IntroVideoUrl { get; set; }

    }
}
