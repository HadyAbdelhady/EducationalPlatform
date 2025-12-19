namespace Application.DTOs.Questions
{
    public class AddQuestionToExamBankResponse
    {
        public Guid CourseId { get; set; }
        public Guid? SectionId { get; set; }

        public Guid ExamId { get; set; }
    }
}
