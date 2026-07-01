using System.Text.Json.Serialization;

namespace Application.DTOs.Payment.PaymobRawDtos
{
    public class CreatePaymentIntentionRequest
    {
        /// <summary>
        /// The total amount in the smallest currency unit (e.g., cents or piastres).
        /// </summary>
        [JsonPropertyName("amount")]
        public required decimal Amount { get; set; }

        /// <summary>
        /// The currency code (e.g., "EGP", "USD").
        /// </summary>
        [JsonPropertyName("currency")]
        public required string Currency { get; set; }

        /// <summary>
        /// Billing information for the customer.
        /// </summary>
        [JsonPropertyName("billing_data")]
        public BillingData BillingData { get; set; } 

        /// <summary>
        /// List of payment method integration IDs allowed for this intention.
        /// </summary>
        [JsonPropertyName("payment_methods")]
        public required List<int> PaymentMethods { get; set; }

        /// <summary>
        /// List of items included in the order.
        /// </summary>
        [JsonPropertyName("items")]
        public List<OrderItem> Items { get; set; } = [];


        /// <summary>
        /// Optional extra data passed to the gateway.
        /// </summary>
        [JsonPropertyName("extras")]
        public Extras Extras { get; set; } = new Extras();

        /// <summary>
        /// A unique reference ID for your internal tracking.
        /// </summary>
        [JsonPropertyName("special_reference")]
        public string SpecialReference { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Time in seconds until the intention expires.
        /// </summary>
        [JsonPropertyName("expiration")]
        public int? Expiration { get; set; } = 3600; // Default to 1 hour

        /// <summary>
        /// URL to receive webhook notifications about payment status changes.
        /// </summary>
        [JsonPropertyName("notification_url")]
        public string NotificationUrl { get; set; } = "https://www.google.com/"; // Replace with your actual success URL

        /// <summary>
        /// URL to redirect the user after payment completion.
        /// </summary>
        [JsonPropertyName("redirection_url")]
        public string RedirectionUrl { get; set; } = "https://www.google.com/"; // Replace with your actual success URL
    }
}