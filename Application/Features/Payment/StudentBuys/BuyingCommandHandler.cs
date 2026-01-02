using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using Domain.Events;
using MediatR;

namespace Application.Features.Payment.StudentBuys
{
    public class BuyingCommandHandler(IUnitOfWork unitOfWork, IMediator mediator) : IRequestHandler<BuyingCommand, Result<StudentBuyResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMediator _mediator = mediator;

        public async Task<Result<StudentBuyResponse>> Handle(BuyingCommand request, CancellationToken cancellationToken)
        {

            PaymentInitiatedEventDto paymentData = new()
            {
                //PaymentId = Guid.NewGuid(),
                StudentId = request.StudentId,
                EntityId = request.EntityId,
                EntityType = request.EntityToBuy == EntityToBuy.Course ? EntityToBuy.Course : EntityToBuy.Section
            };
            await _mediator.Publish(new PaymentInitiatedEvent(paymentData), cancellationToken);
            var response = new StudentBuyResponse
            {
                StudentId = request.StudentId,
                EntityId = request.EntityId,
                EntityToBuy = request.EntityToBuy
            };
            return Result<StudentBuyResponse>.Success(response);
        }
    }
}
