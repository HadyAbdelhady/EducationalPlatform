using Application.Features.HomeScreen.DTOs;
using Application.Common;
using MediatR;

namespace Application.Features.HomeScreen.InstructorDashboard
{
    public class InstructorDashboardQuery : IRequest<Result<InstructorDashboardResponse>>
    {
        public Guid InstructorId { get; set; }
        public Guid? EducationYearId { get; set; }
    }
}
