using Application.Common.Interfaces;
using Application.Features.EducationYears.Interfaces;
using Infrastructure.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.EducationYears
{
    public class StudentEducationYearProvider(EducationDbContext context) : IStudentEducationYearProvider
    {
        private readonly EducationDbContext _context = context;

        public async Task<Guid?> GetEducationYearIdByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var student = await _context.Students
                .AsNoTracking()
                .Where(s => s.UserId == userId)
                .Select(s => s.EducationYearId)
                .FirstOrDefaultAsync(cancellationToken);

            return student != default ? student : null;
        }
    }
}
