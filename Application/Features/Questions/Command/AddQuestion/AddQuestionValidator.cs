using FluentValidation;

namespace Application.Features.Questions.Command.AddQuestion
{
    public class CreateQuestionValidator : AbstractValidator<AddQuestionCommand>
    {
        public CreateQuestionValidator()
        {
            RuleFor(x => x.QuestionString)
                .NotEmpty().WithMessage("Question text is required.")
                .MaximumLength(500);

            RuleFor(x => x.Answers)
                .NotEmpty().WithMessage("You must provide at least two answers.")
                .Must(a => a.Count >= 2).WithMessage("A question needs at least 2 options.");

            // CRITICAL: Ensure at least one answer is marked correct
            RuleFor(x => x.Answers)
                .Must(a => a.Any(x => x.IsCorrect))
                .WithMessage("At least one answer must be marked as correct.");
        }
    }
}
