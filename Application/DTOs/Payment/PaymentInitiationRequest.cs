using Domain.enums;

namespace Application.DTOs.Payment
{
    public class PaymentInitiationRequest
    {
        public Guid StudentId { get; set; }
        public Guid EntityId { get; set; }
        public EntityType EntityType { get; set; }
        public decimal Amount { get; set; }
        public string SenderAccount { get; set; } = string.Empty;
        public string ReceiverAccount { get; set; } = string.Empty;
    }
}

