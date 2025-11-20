namespace Domain.Entities
{
    public class ExamQuestions
    {
        public Guid Id { get; set; }

        public Guid ExamId { get; set; }
        public Guid QuestionId { get; set; }

        // Navigation Properties
        public Exam Exam { get; set; } = null!;
        public Question Question { get; set; } = null!;
    }

}
