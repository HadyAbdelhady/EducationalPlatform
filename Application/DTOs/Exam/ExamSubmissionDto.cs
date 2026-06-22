using Application.ResultWrapper;
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
        public int PassMarkPercentage { get; set; }
        public decimal? Percentage => ObtainedMarks.HasValue && TotalMark > 0 ? (ObtainedMarks.Value / TotalMark) * 100 : null;

        public DateTimeOffset? TakenAt { get; set; }
        public DateTimeOffset SubmittedAt { get; set; }

        public bool IsPassed => Percentage.HasValue && Percentage >= PassMarkPercentage;

        public int NumberOfAnswersSubmitted { get; set; }
        public int TotalQuestions { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class ExamDetails
    {
        public Guid ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public decimal TotalMark { get; set; }
        public int NumberOfQuestions { get; set; }
        public string? CourseName { get; set; } = string.Empty;
        public string? SectionName { get; set; } = string.Empty;
        public string EducatunalYearName { get; set; } = string.Empty;
        public bool IsRandomized { get; set; }
        public ExamType ExamType { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
    }

    public class ExamSubmissionsListResponse
    {
        public ExamDetails Exam { get; set; } = new();
        public PaginatedResult<ExamSubmissionDto> Submissions { get; set; } = new();
    }
}
