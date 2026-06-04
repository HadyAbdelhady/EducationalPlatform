using Application.DTOs.HomeScreen;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.HomeScreen.StudentProgress
{
    public class StudentProgressQuery : IRequest<Result<StudentEnrollmentProgressResponse>>
    {
        public Guid StudentId { get; set; }
        public int EnrollmentsPage { get; set; } = 1;
        public int EnrollmentsPageSize { get; set; } = 6;
        public int MilestonesPage { get; set; } = 1;
        public int MilestonesPageSize { get; set; } = 10;
    }
}
