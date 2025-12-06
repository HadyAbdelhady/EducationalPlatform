namespace Application.DTOs.Answer
{
    public record UpdateAnswerDto
    {
        public Guid? Id { get; set; } 
        public string AnswerText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public string? Explanation { get; set; }
    }
}
