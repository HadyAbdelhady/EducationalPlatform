using Domain.Interfaces;

namespace Domain.Entities
{

    public class StudentSubmission : ISoftDeletableEntity
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }
        public Guid ExamResultId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid? ChosenAnswerId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public Question Question { get; set; } = null!;
        public Answer? ChosenAnswer { get; set; }
        public StudentExamResult ExamResult { get; set; } = null!;

        public Student Student { get; set; } = null!;
    }
}