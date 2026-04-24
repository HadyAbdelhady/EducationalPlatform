using Application.DTOs.EducationYear;
using FluentValidation;

namespace Application.Features.EducationYears.Commands.UpdateEducationYear
{
    public class UpdateEducationYearCommandValidator : AbstractValidator<UpdateEducationYearCommand>
    {
        public UpdateEducationYearCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Education year ID is required.");

            RuleFor(x => x.EducationYear.EducationYearName)
                .NotEmpty().WithMessage("Education year name is required.")
                .Length(2, 100).WithMessage("Education year name must be between 2 and 100 characters.")
                .Matches(@"^[a-zA-Z0-9\s\-]+$").WithMessage("Education year name can only contain letters, numbers, spaces, and hyphens.");
        }
    }
}
