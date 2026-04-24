using Application.DTOs.EducationYear;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.EducationYears.Commands.UpdateEducationYear
{
    public class UpdateEducationYearCommand : IRequest<Result<EducationYearResponse>>
    {
        public Guid Id { get; set; }
        public UpdateEducationYearRequest EducationYear { get; set; } = new();
    }
}
