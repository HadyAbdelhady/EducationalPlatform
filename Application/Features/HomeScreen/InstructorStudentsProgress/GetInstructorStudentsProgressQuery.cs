using Application.DTOs.HomeScreen;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.HomeScreen.InstructorStudentsProgress
{
    public class GetInstructorStudentsProgressQuery : IRequest<Result<InstructorStudentsProgressResponse>>
    {
        public Guid InstructorId { get; set; }

        public Guid? CourseId { get; set; }

        public Guid? SectionId { get; set; }

        public Guid? StudentId { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
