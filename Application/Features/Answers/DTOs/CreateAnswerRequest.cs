namespace Application.Features.Answers.DTOs
{
    public record CreateAnswerDto
    {
        public string AnswerText { get; init; } = string.Empty;
        public bool IsCorrect { get; init; }
    }
}
