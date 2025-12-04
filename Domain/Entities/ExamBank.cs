using Domain.Interfaces;

namespace Domain.Entities
{
    public class ExamBank : ISoftDeletableEntity
    {
        public Guid Id { get; set; }

        public Guid ExamId { get; set; }
        public Guid QuestionId { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
        
        // Navigation Properties
        public Exam Exam { get; set; } = null!;
        public Question Question { get; set; } = null!;
    }

}
