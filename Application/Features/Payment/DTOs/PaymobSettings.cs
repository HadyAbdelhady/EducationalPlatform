using System.Runtime;

namespace Application.Features.Payment.DTOs
{
    public class PaymobSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string PublicTestKey { get; set; } = string.Empty;
        public string SecretTestKey { get; set; } = string.Empty;
        public string Hmac { get; set; } = string.Empty;  // ← PUBLIC
        public string BaseUrl { get; set; } = string.Empty;
        public string NotificationUrl { get; set; } = string.Empty;
        public string RedirectionUrl { get; set; } = string.Empty;

        // Keep getters if you want, but they're not needed now
        public string GetPublicKey() => PublicTestKey;
        public string GetHmac() => Hmac;
        public string GetNotificationUrl() => NotificationUrl;
        public string GetRedirectionUrl() => RedirectionUrl;
    }
}
