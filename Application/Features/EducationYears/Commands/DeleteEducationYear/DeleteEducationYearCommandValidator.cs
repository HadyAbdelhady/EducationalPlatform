using FluentValidation;

namespace Application.Features.EducationYears.Commands.DeleteEducationYear
{
    public class DeleteEducationYearCommandValidator : AbstractValidator<DeleteEducationYearCommand>
    {
        public DeleteEducationYearCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Education year ID is required.");
        }
    }
}
