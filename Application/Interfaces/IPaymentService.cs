using Application.DTOs.Payment;
using Application.ResultWrapper;

namespace Application.Interfaces
{
    public interface IPaymentService
    {
        Task<Result<PaymentResponse>> InitiatePaymentAsync(PaymentInitiationRequest request, CancellationToken cancellationToken = default);
        Task<Result<PaymentResponse>> ProcessPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default);
        Task<Result<PaymentResponse>> GetPaymentByIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<PaymentResponse>>> GetPaymentsByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    }
}

