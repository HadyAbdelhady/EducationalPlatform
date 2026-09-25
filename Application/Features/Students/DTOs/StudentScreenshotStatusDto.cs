namespace Application.Features.Students.DTOs
{
    public class StudentScreenshotStatusDto
    {
        public Guid StudentId { get; set; }
        public int ScreenshotTrials { get; set; }
        public bool TriedScreenshot { get; set; }
        public string? EntityType { get; set; }
        public Guid? EntityId { get; set; }
        public string? PageName { get; set; }
        public string? ScreenshotUrl { get; set; }
        public DateTimeOffset? AttemptedAt { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
