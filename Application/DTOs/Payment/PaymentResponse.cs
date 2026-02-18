using Domain.enums;

namespace Application.DTOs.Payment
{
    public class PaymentResponse
    {
        public Guid PaymentId { get; set; }
        public Guid StudentId { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? SectionId { get; set; }
        public PaymentStatus Status { get; set; }
        public decimal Amount { get; set; }
        public string SenderAccount { get; set; } = string.Empty;
        public string ReceiverAccount { get; set; } = string.Empty;
        public string? CommissionAccount1 { get; set; }
        public string? CommissionAccount2 { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}

