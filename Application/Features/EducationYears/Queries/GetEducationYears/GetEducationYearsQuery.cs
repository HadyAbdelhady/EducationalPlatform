using Application.DTOs.EducationYear;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.EducationYears.Queries.GetEducationYears
{
    public class GetEducationYearsQuery : IRequest<Result<List<EducationYearDto>>>
    {
        public Guid? InstructorId { get; set; }
        public string? ApplicationName { get; set; }

    }
}
