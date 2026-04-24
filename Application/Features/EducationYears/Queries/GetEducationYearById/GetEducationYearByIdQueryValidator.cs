using FluentValidation;

namespace Application.Features.EducationYears.Queries.GetEducationYearById
{
    public class GetEducationYearByIdQueryValidator : AbstractValidator<GetEducationYearByIdQuery>
    {
        public GetEducationYearByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Education year ID is required.");
        }
    }
}
