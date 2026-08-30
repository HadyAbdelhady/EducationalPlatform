using Domain.enums;
using System.Text.Json.Serialization;

namespace Application.Features.Exams.DTOs
{
    public class LiveExamResponse
    {
        public Guid ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int NotStartedCount { get; set; }
        public int InProgressCount { get; set; }
        public int FinishedCount { get; set; }
        public List<LiveExamStudentDto> Students { get; set; } = [];
    }

    public class LiveExamStudentDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExamResultStatus Status { get; set; }

        public DateTimeOffset? TakenAt { get; set; }
        public int? RemainingMinutes { get; set; }
        public decimal? AutoScore { get; set; }
        public bool TriedScreenshot { get; set; }
    }
}
