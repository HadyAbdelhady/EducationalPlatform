using FluentValidation;

namespace Application.Features.Questions.Command.AddQuestion
{
    public class BulkAddQuestionsValidator : AbstractValidator<BulkAddQuestionsCommand>
    {
        public BulkAddQuestionsValidator()
        {
            RuleFor(x => x.Questions)
                .NotEmpty().WithMessage("At least one question is required.");

            RuleForEach(x => x.Questions).ChildRules(item =>
            {
                item.RuleFor(q => q.QuestionString)
                    .NotEmpty().WithMessage("Question text is required.")
                    .MaximumLength(500);

                item.RuleFor(q => q.AnswerTexts)
                    .NotEmpty().WithMessage("Each question must have at least two answers.")
                    .Must(a => a.Count >= 2).WithMessage("A question needs at least 2 options.");

                item.RuleFor(q => q.IsCorrects)
                    .Must(a => a.Any(x => x)).WithMessage("At least one answer must be marked as correct.");
            });
        }
    }
}

