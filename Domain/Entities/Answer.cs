using Domain.Interfaces;

namespace Domain.Entities
{
    public class Answer : ISoftDeletableEntity
    {
        public Guid Id { get; set; }
        public Guid? QuestionId { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; }= DateTimeOffset.UtcNow;
        public bool IsDeleted { get; set; } = false;

        public Question? Question { get; set; }
    }
}


