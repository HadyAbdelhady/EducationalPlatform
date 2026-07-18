using System.Text.Json.Serialization;
using Application.Features.Payment.DTOs.PaymobRawDtos;
using Domain.enums;

namespace Application.Features.Payment.DTOs
{
    public record Money(decimal Amount, string Currency);

    public record Student(string? FirstName, string? LastName, string? Email);
    public class PaymentInitiationRequest
    {
        public Guid EntityId { get; set; }
        public EntityToBuy EntityType { get; set; }
        public Money Money { get; set; } = null!;
        [JsonPropertyName("paymentMethod")]
        public PaymentMethodKeys PaymentMethods { get; set; }
        public List<OrderItem> Items { get; set; } = [];

        [JsonIgnore]
        public Student? Student { get; set; }
    }
}


