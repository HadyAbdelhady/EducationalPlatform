using Application.DTOs.EducationYear;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class EducationYearRepository(EducationDbContext context) : Repository<EducationYear>(context), IEducationYearRepository
    {
        public async Task<List<EducationYearDto>> GetActiveEducationYearsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.EducationYears
                .AsNoTracking()
                .OrderByDescending(ey => ey.CreatedAt)
                .Select(ey => new EducationYearDto
                {
                    Id = ey.Id,
                    EducationYearName = ey.EducationYearName
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<EducationYearDto?> GetEducationYearByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.EducationYears
                .AsNoTracking()
                .Where(ey => ey.Id == id && !ey.IsDeleted)
                .Select(ey => new EducationYearDto
                {
                    Id = ey.Id,
                    EducationYearName = ey.EducationYearName
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.EducationYears
                .AsNoTracking()
                .AnyAsync(ey => ey.EducationYearName.Equals(name, StringComparison.OrdinalIgnoreCase) && !ey.IsDeleted, cancellationToken);
        }

        public async Task<bool> ExistsByNameAndIdAsync(string name, Guid excludeId, CancellationToken cancellationToken = default)
        {
            return await _context.EducationYears
                .AsNoTracking()
                .AnyAsync(ey => ey.Id != excludeId &&
                               ey.EducationYearName.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                               !ey.IsDeleted, cancellationToken);
        }
    }
}
