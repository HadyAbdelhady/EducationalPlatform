using Application.DTOs.HomeScreen;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.HomeScreen.StudentProgress
{
    public class StudentProgressQuery : IRequest<Result<StudentProgressSummaryResponse>>
    {
        public Guid StudentId { get; set; }
        public int CoursesPage { get; set; } = 1;
        public int CoursesPageSize { get; set; } = 6;
        public int MilestonesPage { get; set; } = 1;
        public int MilestonesPageSize { get; set; } = 10;
    }
}
