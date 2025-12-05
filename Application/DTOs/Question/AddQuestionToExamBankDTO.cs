namespace Application.DTOs.Question
{
    public class AddQuestionToExamBankDTO
    {
        public Guid ExamId { get; set; }
        public Guid QuestionId { get; set; }
    }
}
