using Application.DTOs.Payment;

namespace Application.Interfaces
{
    public interface IPaymentRepository : IRepository<Domain.Entities.Payment>
    {
        Task<IEnumerable<PaymentResponse>> GetPaymentsByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<PaymentResponse?> GetPaymentByIdWithDetailsAsync(Guid paymentId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PaymentResponse>> GetPaymentsByStatusAsync(Domain.enums.PaymentStatus status, CancellationToken cancellationToken = default);
    }
}

