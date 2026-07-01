namespace Application.DTOs.Payment.PaymobRawDtos
{
    public class PaymentWebhookRequest
    {
        public string PublicKey { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
    }
}