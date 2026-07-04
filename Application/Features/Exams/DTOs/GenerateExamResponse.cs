namespace Application.Features.Exams.DTOs
{
    public class GenerateExamResponse
    {
        public string Message { get; set; } = string.Empty;
        public Guid ExamId { get; set; }
    }
}
