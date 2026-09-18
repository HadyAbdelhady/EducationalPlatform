using Application.Common;
using MediatR;

namespace Application.Features.Payment.Refund
{
    public record RefundCommand(Guid PaymentId, Guid StudentId) : IRequest<Result<bool>>;
}
