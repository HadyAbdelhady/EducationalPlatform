namespace Application.Features.Answers.DTOs
{
    public class AnswerDto
    {
        public Guid Id { get; set; }
        public string AnswerString { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public Guid QuestionId { get; set; }
    }
}
