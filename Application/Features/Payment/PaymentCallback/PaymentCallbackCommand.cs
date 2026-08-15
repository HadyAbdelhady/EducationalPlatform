using Application.Common;
using MediatR;

namespace Application.Features.Payment.PaymentCallback
{
    public class PaymentCallbackCommand : IRequest<Result<bool>>
    {
        public Guid PaymentId { get; set; }
        public bool Success { get; set; }
    }
}
