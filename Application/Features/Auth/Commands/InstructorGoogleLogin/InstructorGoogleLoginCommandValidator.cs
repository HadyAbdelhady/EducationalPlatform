using FluentValidation;

namespace Application.Features.Auth.Commands.InstructorGoogleLogin
{
    /// <summary>
    /// Validator for InstructorGoogleLoginCommand to ensure all required fields are provided correctly.
    /// </summary>
    public class InstructorGoogleLoginCommandValidator : AbstractValidator<InstructorGoogleLoginCommand>
    {
        public InstructorGoogleLoginCommandValidator()
        {
            RuleFor(x => x.IdToken)
                .NotEmpty()
                .WithMessage("Google ID token is required.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Phone number must be in a valid format.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .WithMessage("Date of birth is required.")
                .Must(BeAValidAge)
                .WithMessage("Instructor must be at least 18 years old.");

            RuleFor(x => x.Gender)
                .NotEmpty()
                .WithMessage("Gender is required.")
                .Must(g => g == "Male" || g == "Female")
                .WithMessage("Gender must be Male, Female.");

            //RuleFor(x => x.EducationYear)
            //    .NotEmpty()
            //    .WithMessage("Education year/qualification is required.");
        }

        private bool BeAValidAge(DateOnly dateOfBirth)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth > today.AddYears(-age)) age--;
            return age >= 18;
        }
    }
}
