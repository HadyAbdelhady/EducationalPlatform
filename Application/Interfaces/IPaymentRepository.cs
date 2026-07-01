using Application.DTOs.Payment.PaymobRawDtos;

namespace Application.Interfaces
{
    public interface IPaymentRepository : IRepository<Domain.Entities.PaymentTransactions>
    {
        Task<IEnumerable<PaymentResponse>> GetPaymentsByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<PaymentResponse?> GetPaymentByIdWithDetailsAsync(Guid paymentId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PaymentResponse>> GetPaymentsByStatusAsync(Domain.enums.PaymentStatus status, CancellationToken cancellationToken = default);
    }
}

