using Application.Common;
using Application.Features.HomeScreen.DTOs;
using MediatR;

namespace Application.Features.HomeScreen.InstructorSchedule
{
    public class InstructorScheduleQuery : IRequest<Result<InstructorScheduleResponse>>
    {
        public Guid InstructorId { get; set; }
        public Guid EducationYearId { get; set; }
        public int Days { get; set; } = 7;
    }
}
