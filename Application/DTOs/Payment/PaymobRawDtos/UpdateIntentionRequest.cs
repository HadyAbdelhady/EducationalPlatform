using System.Text.Json.Serialization;

namespace Application.DTOs.Payment.PaymobRawDtos
{
    public class UpdateIntentionRequest
    {
        /// <summary>
        /// The total amount of the intention in the smallest currency unit (e.g., cents or piastres).
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// List of payment method integration IDs allowed for this intention.
        /// </summary>
        [JsonPropertyName("payment_methods")]
        public List<int> PaymentMethods { get; set; }

        /// <summary>
        /// List of items included in the order.
        /// </summary>
        [JsonPropertyName("items")]
        public List<OrderItem> Items { get; set; }

        /// <summary>
        /// Billing information for the customer.
        /// </summary>
        [JsonPropertyName("billing_data")]
        public BillingData BillingData { get; set; }

        /// <summary>
        /// A unique reference ID for your internal tracking.
        /// </summary>
        [JsonPropertyName("special_reference")]
        public string SpecialReference { get; set; }

        /// <summary>
        /// Time in seconds until the intention expires.
        /// </summary>
        [JsonPropertyName("expiration")]
        public int? Expiration { get; set; }

        /// <summary>
        /// URL to receive webhook notifications about payment status changes.
        /// </summary>
        [JsonPropertyName("notification_url")]
        public string NotificationUrl { get; set; }

        /// <summary>
        /// URL to redirect the user after payment completion.
        /// </summary>
        [JsonPropertyName("redirection_url")]
        public string RedirectionUrl { get; set; }
    }

}