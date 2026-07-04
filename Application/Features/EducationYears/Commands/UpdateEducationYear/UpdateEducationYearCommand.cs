using Application.Features.EducationYears.DTOs;
using Application.Common;
using MediatR;

namespace Application.Features.EducationYears.Commands.UpdateEducationYear
{
    public class UpdateEducationYearCommand : IRequest<Result<EducationYearResponse>>
    {
        public Guid Id { get; set; }
        public UpdateEducationYearRequest EducationYear { get; set; } = new();
    }
}
