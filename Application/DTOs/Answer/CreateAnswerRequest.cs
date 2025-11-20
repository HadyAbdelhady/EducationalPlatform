namespace Application.DTOs.Answer
{
    public record CreateAnswerDto
    {
        public string AnswerText { get; init; } = string.Empty;
        public bool IsCorrect { get; init; }
    }
}
