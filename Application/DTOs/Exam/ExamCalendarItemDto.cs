namespace Application.DTOs.Exam
{
    public class ExamCalendarItemDto
    {
        public string Date { get; set; } = string.Empty; // yyyy-MM-dd
        public string Type { get; set; } = string.Empty; // "Course" | "Section"
        public string ExamName { get; set; } = string.Empty;
    }
}

