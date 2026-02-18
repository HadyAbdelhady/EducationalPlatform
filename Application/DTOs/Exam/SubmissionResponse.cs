namespace Application.DTOs.Exam
{
    public record SubmissionResponse
    {
        public string StudentName { get; set; } = string.Empty;
        public string ExamName { get; set; } = string.Empty;
        public decimal TotalMark { get; set; }
        public decimal ObtainedMark { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
        public bool IsSuccessful { get; set; }

    }
}
