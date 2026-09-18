using Application.Features.Payment.Interfaces;
using Domain.Entities;
using Infrastructure.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.Payment.PayoutRepository
{
    public class PayoutRepository(EducationDbContext context) : IPayoutRepository
    {
        private readonly EducationDbContext _context = context;

        public Task<List<Center>> GetAllActiveCentersAsync(CancellationToken ct = default)
            => _context.Centers.AsNoTracking().ToListAsync(ct);

        public Task<List<Instructor>> GetAllIndependentInstructorsAsync(CancellationToken ct = default)
        {
            return _context.Instructors
                .AsNoTracking()
                .Where(i => !i.CenterInstructors.Any())
                .ToListAsync(ct);
        }
    }
}
