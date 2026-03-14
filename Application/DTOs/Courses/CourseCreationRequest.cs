using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Courses
{
    public class CourseCreationRequest
    {
        public string CourseName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid EducationYearId { get; set; }
        public Guid InstructorId { get; set; }
        public decimal? Price { get; set; }
        public string? PictureUrl { get; set; }
        public string? IntroVideoUrl { get; set; }
        public IFormFile? PictureFile { get; set; }
    }
}
