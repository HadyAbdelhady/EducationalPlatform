namespace Domain.Entities
{
    public class ExamBank
    {
        public Guid ExamId { get; set; }
        public Guid QuestionId { get; set; }
        public decimal QuestionMark { get; set; }


        // Navigation Properties
        public Exam Exam { get; set; } = null!;
        public Question Question { get; set; } = null!;
    }

}
