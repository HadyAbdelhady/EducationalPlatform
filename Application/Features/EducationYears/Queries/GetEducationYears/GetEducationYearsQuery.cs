using Application.Features.EducationYears.DTOs;
using Application.Common;
using MediatR;

namespace Application.Features.EducationYears.Queries.GetEducationYears
{
    public class GetEducationYearsQuery : IRequest<Result<List<EducationYearDto>>>
    {
        public Guid InstructorId { get; set; }
    }
}
