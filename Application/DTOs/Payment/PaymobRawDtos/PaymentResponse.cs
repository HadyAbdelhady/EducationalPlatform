using Domain.enums;
using System.Text.Json.Serialization;

namespace Application.DTOs.Payment.PaymobRawDtos
{
    public class PaymentIntentionResponse
    {
        [JsonPropertyName("payment_keys")]
        public List<PaymentKey> PaymentKeys { get; set; }

        [JsonPropertyName("intention_order_id")]
        public long IntentionOrderId { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("intention_detail")]
        public IntentionDetail IntentionDetail { get; set; }

        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; }

        [JsonPropertyName("payment_methods")]
        public List<PaymentMethod> PaymentMethods { get; set; }

        [JsonPropertyName("special_reference")]
        public string SpecialReference { get; set; }

        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }

        [JsonPropertyName("confirmed")]
        public bool Confirmed { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }

        // Using object here since it's null in the payload. 
        // You can replace this with a specific class if you know its structure.
        [JsonPropertyName("card_detail")]
        public object CardDetail { get; set; }

        [JsonPropertyName("card_tokens")]
        public List<object> CardTokens { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }
    }

    public class PaymentKey
    {
        [JsonPropertyName("integration")]
        public int Integration { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("gateway_type")]
        public string GatewayType { get; set; }

        [JsonPropertyName("iframe_id")]
        public int? IframeId { get; set; }

        [JsonPropertyName("order_id")]
        public long OrderId { get; set; }
    }

    public class IntentionDetail
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("items")]
        public List<OrderItem> Items { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("billing_data")]
        public BillingData BillingData { get; set; }
    }


    public class PaymentMethod
    {
        [JsonPropertyName("integration_id")]
        public long IntegrationId { get; set; }

        [JsonPropertyName("alias")]
        public string Alias { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("method_type")]
        public string MethodType { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("live")]
        public bool Live { get; set; }

        [JsonPropertyName("use_cvc_with_moto")]
        public bool UseCvcWithMoto { get; set; }
    }

    public class PaymentResponse
    {
        public Guid PaymentId { get; set; }
        public Guid StudentId { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? SectionId { get; set; }
        public PaymentStatus Status { get; set; }
        public decimal Amount { get; set; }
        public string SenderAccount { get; set; }
        public string ReceiverAccount { get; set; }
        public string? CommissionAccount1 { get; set; }
        public string? CommissionAccount2 { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}