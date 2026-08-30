namespace Application.Features.HomeScreen.DTOs
{
    public class AtRiskResponse
    {
        public List<AtRiskStudentDto> Students { get; set; } = [];
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class AtRiskStudentDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string? PictureUrl { get; set; }
        public string ParentPhone { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty; // Inactive | FailedExams | OverdueSheet
        public DateTimeOffset? LastSeenAt { get; set; }
        public int FailedExamCount { get; set; }
        public int OverdueSheetCount { get; set; }
    }
}
