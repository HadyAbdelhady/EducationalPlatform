using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Auth.Interfaces;
using Application.Features.HomeScreen.Interfaces;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Payment.PaymentCallback
{
    public class PaymentCallbackCommandHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<PaymentCallbackCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<bool>> Handle(PaymentCallbackCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var payment = await _unitOfWork.Repository<PaymentTransactions>()
                    .FirstOrDefaultAsync(x => x.Id == request.PaymentId, cancellationToken);

                if (payment is null)
                {
                    return Result<bool>.FailureStatusCode("Payment not found", ErrorType.NotFound);
                }

                if (payment.Status == PaymentStatus.Completed)
                    return Result<bool>.Success(true);

                if (!request.Success)
                {
                    payment.Status = PaymentStatus.Failed;
                    payment.UpdatedAt = DateTimeOffset.UtcNow;
                    _unitOfWork.Repository<PaymentTransactions>().Update(payment);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<bool>.FailureStatusCode("Payment failed", ErrorType.BadRequest);
                }

                payment.Status = PaymentStatus.Completed;
                payment.UpdatedAt = DateTimeOffset.UtcNow;

                var studentExists = await _unitOfWork.GetRepository<IUserRepository>()
                    .DoesStudentExistAsync(payment.StudentId, cancellationToken);

                if (!studentExists)
                {
                    return Result<bool>.FailureStatusCode("Student not found", ErrorType.NotFound);
                }

                var enrollmentRepo = _unitOfWork.GetRepository<IStudentEnrollmentRepository>();
                await enrollmentRepo.EnrollFromPaymentAsync(
                    payment.StudentId,
                    payment.CourseId,
                    payment.SectionId,
                    cancellationToken);

                _unitOfWork.Repository<PaymentTransactions>().Update(payment);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.FailureStatusCode(
                    $"An error occurred while processing the payment callback: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
