using Application.DTOs.Payment;
using Application.DTOs.Payment.PaymobRawDtos;

namespace Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentIntentionResponse> CreateIntentionAsync(PaymentInitiationRequest request, CancellationToken cancellationToken = default);
        bool VerifyHmacSignature(string concatenatedHmacString, string HmacSignature);
        string GetPublicKey();
    }
}

