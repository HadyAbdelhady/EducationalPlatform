using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Course.Commands.UpdateCourse
{
    public class UpdateCourseCommand: IRequest<Result<string>>
    {
        public Guid Id { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid InstructorId { get; set; }
        public decimal Price { get; set; }
        public string PictureUrl { get; set; } = string.Empty;
        public string IntroVideoUrl { get; set; } = string.Empty;
    }
}
