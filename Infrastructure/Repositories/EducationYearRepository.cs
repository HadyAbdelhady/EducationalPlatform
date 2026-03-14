using Application.DTOs.EducationYear;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class EducationYearRepository(EducationDbContext context) : IEducationYearRepository
    {
        private readonly EducationDbContext _context = context;

        public async Task<List<EducationYearDto>> GetActiveEducationYearsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.EducationYears
                .AsNoTracking()
                .Where(ey => !ey.IsDeleted)
                .OrderByDescending(ey => ey.CreatedAt)
                .Select(ey => new EducationYearDto
                {
                    Id = ey.Id,
                    EducationYearName = ey.EducationYearName
                })
                .ToListAsync(cancellationToken);
        }
    }
}
