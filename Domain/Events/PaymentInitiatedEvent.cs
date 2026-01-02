using Domain.enums;
using MediatR;

namespace Domain.Events
{
    public class PaymentInitiatedEventDto
    {
        //public Guid PaymentId { get; set; }
        public Guid StudentId { get; set; }
        public Guid EntityId { get; set; }
        public EntityToBuy EntityType { get; set; }
    }
    public record PaymentInitiatedEvent(PaymentInitiatedEventDto PaymentData) : INotification;
}

