using Domain.Interfaces;

namespace Domain.Entities
{
    public class Question : ISoftDeletableEntity
    {
        public Guid Id { get; set; }
        public Guid ExamQuestionId { get; set; }
        public string QuestionString { get; set; } = string.Empty;
        public string? QuestionImageUrl { get; set; }
        public decimal? QuestionMark { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<ExamQuestions> ExamQuestions { get; set; } = [];
        public ICollection<Answer> Answers { get; set; } = [];
        public ICollection<StudentSubmission> StudentSubmissions { get; set; } = [];
    }
}


