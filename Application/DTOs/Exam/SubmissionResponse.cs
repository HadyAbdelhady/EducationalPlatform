namespace Application.DTOs.Exam
{
    public record SubmissionResponse
    {
        public Guid StudentName { get; set; }
        public Guid ExamName { get; set; }
        public decimal TotalMark { get; set; }
        public decimal ObtainedMark { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
        public bool IsSuccessful { get; set; }

    }
}
