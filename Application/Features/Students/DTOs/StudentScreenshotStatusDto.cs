namespace Application.Features.Students.DTOs
{
    public class StudentScreenshotStatusDto
    {
        public Guid StudentId { get; set; }
        public int ScreenshotTrials { get; set; }
        public bool TriedScreenshot { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
