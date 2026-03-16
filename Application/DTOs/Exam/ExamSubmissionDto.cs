using Domain.enums;
using System.Text.Json.Serialization;

namespace Application.DTOs.Exam
{
    public class ExamSubmissionDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExamResultStatus Status { get; set; }
        
        public decimal? ObtainedMarks { get; set; }
        public decimal TotalMark { get; set; }
        public decimal? Percentage => ObtainedMarks.HasValue && TotalMark > 0 ? (ObtainedMarks.Value / TotalMark) * 100 : null;
        
        public DateTimeOffset? TakenAt { get; set; }
        public DateTimeOffset SubmittedAt { get; set; }
        
        public bool IsPassed => ObtainedMarks.HasValue && TotalMark > 0 && ObtainedMarks.Value >= (TotalMark * 0.6m); // Assuming 60% pass mark
        
        public int NumberOfAnswersSubmitted { get; set; }
        public int TotalQuestions { get; set; }
        public bool IsCompleted { get; set; }
    }
}
