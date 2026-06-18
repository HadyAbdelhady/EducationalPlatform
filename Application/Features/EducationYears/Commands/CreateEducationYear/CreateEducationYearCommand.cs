using Application.DTOs.EducationYear;
using Application.ResultWrapper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.EducationYears.Commands.CreateEducationYear
{
    public class CreateEducationYearCommand : IRequest<Result<EducationYearResponse>>
    {
        public CreateEducationYearRequest EducationYear { get; set; } = new();

        [Required(ErrorMessage = "Instructor ID is required.")]
        public Guid InstructorId { get; set; }
    }
}
