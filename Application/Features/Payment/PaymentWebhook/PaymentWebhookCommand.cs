using Application.DTOs.Payment.PaymobRawDtos;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Payment.PaymentWebhook
{
    public class PaymentWebhookCommand : IRequest<Result<bool>>
    {
        public PaymobWebhookPayload Payload { get; set; } = null!;
        public string HmacSignature { get; set; } = string.Empty;
    }
}
