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
    }

    public class StudentBuyResponse
    {
        public Guid StudentId { get; set; }
        public Guid EntityId { get; set; }

        public EntityToBuy EntityToBuy { get; set; }

    }

}
