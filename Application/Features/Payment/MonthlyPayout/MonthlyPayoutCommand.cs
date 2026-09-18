using Application.Common;
using MediatR;

namespace Application.Features.Payment.MonthlyPayout
{
    public record MonthlyPayoutCommand : IRequest<Result<bool>>;
}
