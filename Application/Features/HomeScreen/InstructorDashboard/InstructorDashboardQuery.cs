using Application.DTOs.HomeScreen;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.HomeScreen.InstructorDashboard
{
    public class InstructorDashboardQuery : IRequest<Result<InstructorDashboardResponse>>
    {
        public Guid InstructorId { get; set; }
        public Guid? EducationYearId { get; set; }
    }
}
