using Application.DTOs.Payment;
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

        public Money Money { get; set; } = null!;
        
        public bool PaymentMethod { get; set; }
        public Student Student { get; set; } = null!;

    }

    public class StudentBuyResponse
    {
        public Guid StudentId { get; set; }
        public Guid EntityId { get; set; }
        public EntityToBuy EntityToBuy { get; set; }
        public PaymentData PamobData { get; set; } = null!;

    }
    public class PaymentData
    {
        public string PaymentId { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
    }

}
