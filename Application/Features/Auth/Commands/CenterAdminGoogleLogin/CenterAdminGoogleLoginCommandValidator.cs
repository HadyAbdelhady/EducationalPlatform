using FluentValidation;

namespace Application.Features.Auth.Commands.CenterAdminGoogleLogin
{
    public class CenterAdminGoogleLoginCommandValidator : AbstractValidator<CenterAdminGoogleLoginCommand>
    {
        public CenterAdminGoogleLoginCommandValidator()
        {
            RuleFor(x => x.CenterId)
                .NotEmpty().WithMessage("CenterId is required.");

            RuleFor(x => x.GoogleUserInfo)
                .NotNull().WithMessage("Google user info is required.");

            When(x => x.GoogleUserInfo != null, () =>
            {
                RuleFor(x => x.GoogleUserInfo.IdToken)
                    .NotEmpty().WithMessage("IdToken is required.");

                RuleFor(x => x.GoogleUserInfo.Email)
                    .NotEmpty().WithMessage("Email is required.")
                    .EmailAddress().WithMessage("A valid email is required.");

                RuleFor(x => x.GoogleUserInfo.FullName)
                    .NotEmpty().WithMessage("Full Name is required.");

                RuleFor(x => x.GoogleUserInfo.PhoneNumber)
                    .NotEmpty().WithMessage("Phone Number is required.");

                RuleFor(x => x.GoogleUserInfo.Gender)
                    .NotEmpty().WithMessage("Gender is required.");
            });

            RuleFor(x => x.Ssn)
                .NotEmpty().WithMessage("SSN is required.")
                .Length(14).WithMessage("SSN must be exactly 14 characters long.")
                .Matches("^[0-9]+$").WithMessage("SSN must contain only digits.");
        }
    }
}
