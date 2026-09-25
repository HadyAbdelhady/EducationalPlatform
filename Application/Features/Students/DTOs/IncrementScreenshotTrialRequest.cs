namespace Application.Features.Students.DTOs
{
    public class IncrementScreenshotTrialRequest
    {
        public string? EntityType { get; set; }
        public Guid? EntityId { get; set; }
        public string? PageName { get; set; }
    }
}
