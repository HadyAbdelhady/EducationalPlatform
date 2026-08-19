using Application.Common;
using Application.Features.HomeScreen.DTOs;
using MediatR;

namespace Application.Features.HomeScreen.InstructorStudentExams
{
    public class GetInstructorStudentExamsQuery : IRequest<Result<PaginatedResult<InstructorStudentExamDto>>>
    {
        public Guid InstructorId { get; set; }

        public Guid StudentId { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
