using Application.DTOs.Payment.PaymobRawDtos;
using Domain.enums;

namespace Application.DTOs.Payment
{
    public record Money(decimal Amount, string Currency);

    public record Student(string? FirstName, string? LastName, string? Email);
    public class PaymentInitiationRequest
    {
        public Guid EntityId { get; set; }
        public EntityToBuy EntityType { get; set; }
        public Money Money { get; set; } = null!;
        public PaymentMethodKeys PaymentMethods { get; set; }
        public List<OrderItem> Items { get; set; } = [];
        public Student Student { get; set; } = null!;
    }
}


