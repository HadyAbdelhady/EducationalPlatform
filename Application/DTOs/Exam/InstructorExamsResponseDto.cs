using Application.ResultWrapper;
using Domain.enums;
using System.Text.Json.Serialization;

namespace Application.DTOs.Exam
{
    public class InstructorExamsResponseDto
    {
        public Guid ExamId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal TotalMark { get; set; }
        public int NumberOfQuestions { get; set; }
        public int? DurationInMinutes { get; set; }
        public bool IsRandomized { get; set; }
        public int PassMarkPercentage { get; set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExamType ExamType { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExamStatus ExamStatus { get; set; }
        
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        
        // Course and Section information
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public Guid? SectionId { get; set; }
        public string? SectionName { get; set; }
        
        // Additional stats
        public int StudentCount { get; set; }
        public int PassedCount { get; set; }
        public int FailedCount { get; set; }
        public int NotStartedCount { get; set; }
        public int InProgressCount { get; set; }
    }

    public class CoursesSectionsHashMap
    {
        public Dictionary<Guid, CourseSectionInfo> Courses { get; set; } = [];
    }

    public class CourseSectionInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Dictionary<Guid, SectionInfo> Sections { get; set; } = new();
    }

    public class SectionInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class InstructorExamsResult
    {
        public PaginatedResult<InstructorExamsResponseDto> Exams { get; set; } = new();
        public CoursesSectionsHashMap CoursesSections { get; set; } = new();
    }
}
