using Application.Common;
using Application.Features.HomeScreen.DTOs;
using MediatR;

namespace Application.Features.HomeScreen.InstructorAtRisk
{
    public class InstructorAtRiskQuery : IRequest<Result<AtRiskResponse>>
    {
        public Guid InstructorId { get; set; }
        public Guid EducationYearId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
