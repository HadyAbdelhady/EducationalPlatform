using Application.DTOs.Payment;
using Application.Interfaces;
using Domain.enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PaymentRepository(EducationDbContext context) : Repository<Domain.Entities.Payment>(context), IPaymentRepository
    {
        public async Task<IEnumerable<PaymentResponse>> GetPaymentsByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .Where(p => p.StudentId == studentId && !p.IsDeleted)
                .Select(p => new PaymentResponse
                {
                    PaymentId = p.Id,
                    StudentId = p.StudentId,
                    CourseId = p.CourseId,
                    SectionId = p.SectionId,
                    Status = p.Status,
                    Amount = p.Amount,
                    SenderAccount = p.SenderAccount,
                    ReceiverAccount = p.ReceiverAccount,
                    CommissionAccount1 = p.CommissionAccount1,
                    CommissionAccount2 = p.CommissionAccount2,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<PaymentResponse?> GetPaymentByIdWithDetailsAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .Where(p => p.Id == paymentId && !p.IsDeleted)
                .Select(p => new PaymentResponse
                {
                    PaymentId = p.Id,
                    StudentId = p.StudentId,
                    CourseId = p.CourseId,
                    SectionId = p.SectionId,
                    Status = p.Status,
                    Amount = p.Amount,
                    SenderAccount = p.SenderAccount,
                    ReceiverAccount = p.ReceiverAccount,
                    CommissionAccount1 = p.CommissionAccount1,
                    CommissionAccount2 = p.CommissionAccount2,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<PaymentResponse>> GetPaymentsByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .Where(p => p.Status == status && !p.IsDeleted)
                .Select(p => new PaymentResponse
                {
                    PaymentId = p.Id,
                    StudentId = p.StudentId,
                    CourseId = p.CourseId,
                    SectionId = p.SectionId,
                    Status = p.Status,
                    Amount = p.Amount,
                    SenderAccount = p.SenderAccount,
                    ReceiverAccount = p.ReceiverAccount,
                    CommissionAccount1 = p.CommissionAccount1,
                    CommissionAccount2 = p.CommissionAccount2,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}

