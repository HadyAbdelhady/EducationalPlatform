namespace Application.DTOs.Question
{
    public class QuestionDetailsResponse
    {
        public Guid Id { get; set; }
        public Guid SectionId { get; set; }
        public Guid CourseId { get; set; }
        public string QuestionString { get; set; } = string.Empty;
        public string? QuestionImageUrl { get; set; }
        public decimal? QuestionMark { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public List<AnswerResponse> Answers { get; set; } = [];
    }

    public class AnswerResponse
    {
        public Guid Id { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}

