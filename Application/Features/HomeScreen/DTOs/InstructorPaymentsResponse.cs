namespace Application.Features.HomeScreen.DTOs
{
    public class InstructorPaymentsResponse
    {
        public decimal CompletedAmount { get; set; }
        public int PendingCount { get; set; }
        public int FailedCount { get; set; }
        public int Days { get; set; }
        public List<PaymentRowDto> Rows { get; set; } = [];
    }

    public class PaymentRowDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty; // course or section name
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
    }
}
