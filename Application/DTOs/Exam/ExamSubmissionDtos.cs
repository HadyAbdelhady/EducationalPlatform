namespace Application.DTOs.Exam
{
    public class ExamModelAnswer
    {
        public Guid ExamId { get; set; }

        public string Title { get; set; } = string.Empty;

        public decimal TotalMark { get; set; }
        public int PassMarkPercentage { get; set; }

        public List<QuestionModelAnswer> Questions { get; set; } = [];
    }

    public class QuestionModelAnswer
    {
        public Guid QuestionId { get; set; }
        public Guid CorrectAnswerId { get; set; }
        public decimal QuestionMark { get; set; }

    }
}
