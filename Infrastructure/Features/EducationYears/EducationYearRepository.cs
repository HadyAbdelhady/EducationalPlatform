using Infrastructure.Common;
using Application.Features.EducationYears.DTOs;
using Application.Common.Interfaces;
using Application.Features.EducationYears.Interfaces;
using Domain.Entities;
using Infrastructure.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.EducationYears
{
    public class EducationYearRepository(EducationDbContext context) : Repository<EducationYear>(context), IEducationYearRepository
    {
        public async Task<List<EducationYearDto>> GetActiveEducationYearsForInstructorAsync(Guid? instructorId, string? applicationName = null)
        {
            if (instructorId == null)
            {
                return await _context.Instructors
                    .AsNoTracking()
                    .Where(i => i.ApplicationName == applicationName)
                    .SelectMany(i => i.EducationYears)
                    .OrderByDescending(ey => ey.CreatedAt)
                    .Select(ey => new EducationYearDto
                    {
                        Id = ey.Id,
                        EducationYearName = ey.EducationYearName
                    })
                    .ToListAsync();
            }
            else
            {
                return await _context.EducationYears
                    .AsNoTracking()
                    .Where(I => I.InstructorId == instructorId)
                    .OrderByDescending(ey => ey.CreatedAt)
                    .Select(ey => new EducationYearDto
                    {
                        Id = ey.Id,
                        EducationYearName = ey.EducationYearName
                    })
                    .ToListAsync();
            }
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
