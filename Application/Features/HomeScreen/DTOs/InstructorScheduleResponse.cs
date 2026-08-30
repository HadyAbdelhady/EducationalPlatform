namespace Application.Features.HomeScreen.DTOs
{
    public class InstructorScheduleResponse
    {
        public List<ScheduleDayDto> Days { get; set; } = [];
    }

    public class ScheduleDayDto
    {
        public string Date { get; set; } = string.Empty; // "yyyy-MM-dd"
        public List<ScheduleItemDto> Items { get; set; } = [];
    }

    public class ScheduleItemDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty; // "Exam" | "Sheet"
        public string Title { get; set; } = string.Empty;
        public DateTimeOffset? StartOrDue { get; set; }
        public int? DurationInMinutes { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int EnrolledCount { get; set; }
        public int SubmittedCount { get; set; } // sheets only
    }
}
