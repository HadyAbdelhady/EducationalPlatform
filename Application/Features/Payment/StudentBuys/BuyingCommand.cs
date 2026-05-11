using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Payment.StudentBuys
{
    public class BuyingCommand : IRequest<Result<StudentBuyResponse>>
    {
        public Guid StudentId { get; set; }
        public Guid EntityId { get; set; }
        public EntityToBuy EntityToBuy { get; set; }

        public decimal Amount { get; set; }
        public string SenderAccount { get; set; } = string.Empty;
        public string ReceiverAccount { get; set; } = string.Empty;
    }

    public class StudentBuyResponse
    {
        public Guid StudentId { get; set; }
        public Guid EntityId { get; set; }

        public EntityToBuy EntityToBuy { get; set; }

    }

}
