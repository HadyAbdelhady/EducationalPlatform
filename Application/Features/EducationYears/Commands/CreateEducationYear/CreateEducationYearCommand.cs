using Application.DTOs.EducationYear;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.EducationYears.Commands.CreateEducationYear
{
    public class CreateEducationYearCommand : IRequest<Result<EducationYearResponse>>
    {
        public CreateEducationYearRequest EducationYear { get; set; } = new();
    }
}
