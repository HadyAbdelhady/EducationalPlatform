namespace Application.DTOs.Questions
{
    public class GetAllQuestionsInExamResponse
    {
        public Guid Id { get; set; }
        public string QuestionString { get; set; } = string.Empty;
        public string? QuestionImageUrl { get; set; }
        public decimal? QuestionMark { get; set; }
        public Guid SectionId { get; set; }
        public Guid CourseId { get; set; }
    }
}

