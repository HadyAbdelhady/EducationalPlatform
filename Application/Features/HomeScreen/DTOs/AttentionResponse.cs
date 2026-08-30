namespace Application.Features.HomeScreen.DTOs
{
    public class AttentionResponse
    {
        public List<AttentionItemDto> Items { get; set; } = [];
    }

    public class AttentionItemDto
    {
        public string Type { get; set; } = string.Empty; // SheetPendingApproval | SheetOverdue | ExamFailed | ScreenshotFlag | NewEnrollment
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTimeOffset When { get; set; }
        public string ParentPhone { get; set; } = string.Empty;
    }
}
