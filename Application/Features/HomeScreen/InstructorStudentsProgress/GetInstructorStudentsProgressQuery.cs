using Application.Features.HomeScreen.DTOs;
using Application.Common;
using MediatR;

namespace Application.Features.HomeScreen.InstructorStudentsProgress
{
    public class GetInstructorStudentsProgressQuery : IRequest<Result<InstructorStudentsProgressResponse>>
    {
        public Guid InstructorId { get; set; }

        public Guid? CourseId { get; set; }

        public Guid? SectionId { get; set; }

        public Guid? StudentId { get; set; }

        public string? Search { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
