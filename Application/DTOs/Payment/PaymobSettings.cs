using System.Runtime;

namespace Application.DTOs.Payment
{
    public class PaymobSettings
    {
        private string ApiKey { get; set; } = string.Empty;
        private string PublicTestKey { get; set; } = string.Empty;
        private string SecretTestKey { get; set; } = string.Empty;
        private string Hmac { get; set; } = string.Empty;
        private string BaseUrl { get; set; } = string.Empty;



        public string GetPublicKey() => PublicTestKey;

        public string GetHmac() => Hmac;
    }
}
