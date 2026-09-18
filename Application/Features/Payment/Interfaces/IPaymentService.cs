using Application.Features.Payment.DTOs;
using Application.Features.Payment.DTOs.PaymobRawDtos;

namespace Application.Features.Payment.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentIntentionResponse> CreateIntentionAsync(PaymentInitiationRequest request, CancellationToken cancellationToken = default);
        bool VerifyHmacSignature(string concatenatedHmacString, string HmacSignature);
        string GetPublicKey();
        string GetHmac();
        Task<string?> RefundAsync(string paymobTransactionId, int amountCents, CancellationToken cancellationToken = default);
        Task<string?> SendPayoutAsync(string recipientId, decimal amountEgp, string reference, CancellationToken cancellationToken = default);
    }
}


