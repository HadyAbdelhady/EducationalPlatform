using System;

namespace Domain.Entities
{
    public class Answer
    {
        public Guid Id { get; set; }
        public Guid? QuestionId { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public Question? Question { get; set; }
    }
}


