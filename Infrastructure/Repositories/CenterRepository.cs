using Application.DTOs.Center;
using Application.Interfaces;
using Domain;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CenterRepository(EducationDbContext context)
        : Repository<Center>(context), ICenterRepository
    {
        public async Task<CenterResponse?> GetCenterWithDetailsAsync(Guid centerId, CancellationToken cancellationToken = default)
        {
            return await _context.Centers
                .Where(c => c.Id == centerId)
                .Select(c => new CenterResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    LogoUrl = c.LogoUrl,
                    LocationMaps = c.LocationMaps,
                    CreatedAt = c.CreatedAt,
                    Instructors = c.CenterInstructors
                        .Select(ci => new CenterInstructorDto
                        {
                            InstructorId = ci.InstructorId,
                            FullName = ci.Instructor.User.FullName,
                            EducationYears = ci.Instructor.CenterInstructorYears
                                .Where(y => y.CenterId == centerId && y.IsActive)
                                .Select(y => new CenterEducationYearDto
                                {
                                    EducationYearId = y.EducationYearId,
                                    EducationYearName = y.EducationYear.EducationYearName,
                                    IsActive = y.IsActive
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<CenterSummaryResponse>> GetAllCentersAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Centers
                .Select(c => new CenterSummaryResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    LogoUrl = c.LogoUrl,
                    InstructorCount = c.CenterInstructors.Count(ci => !ci.IsDeleted),
                    CreatedAt = c.CreatedAt
                })
                .OrderBy(c => c.Name)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Guid>> GetInstructorIdsByCenterIdAsync(Guid centerId, CancellationToken cancellationToken = default)
        {
            return await _context.CenterInstructors
                .Where(ci => ci.CenterId == centerId)
                .Select(ci => ci.InstructorId)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsInstructorInCenterAsync(Guid centerId, Guid instructorId, CancellationToken cancellationToken = default)
        {
            return await _context.CenterInstructors
                .AnyAsync(ci => ci.CenterId == centerId &&
                                ci.InstructorId == instructorId,
                          cancellationToken);
        }

        public async Task<Guid?> GetStudentCenterIdAsync(Guid studentUserId, CancellationToken cancellationToken = default)
        {
            var centerId = await _context.Students
                .Where(s => s.UserId == studentUserId)
                .Select(s => s.CenterId)
                .FirstOrDefaultAsync(cancellationToken);

            return centerId;
        }

        public async Task AddInstructorToCenterAsync(
            Guid centerId,
            Guid instructorId,
            IEnumerable<Guid> educationYearIds,
            CancellationToken cancellationToken = default)
        {
            var centerInstructor = new CenterInstructor
            {
                CenterId = centerId,
                InstructorId = instructorId,
                JoinedAt = EgyptTime.UtcNow
            };

            await _context.CenterInstructors.AddAsync(centerInstructor, cancellationToken);

            var yearList = educationYearIds.ToList();
            if (yearList.Count > 0)
            {
                var yearAssignments = yearList.Select(yearId => new CenterInstructorEducationYear
                {
                    CenterId = centerId,
                    InstructorId = instructorId,
                    EducationYearId = yearId,
                    IsActive = true,
                    AssignedAt = EgyptTime.UtcNow
                });

                await _context.CenterInstructorEducationYears.AddRangeAsync(yearAssignments, cancellationToken);
            }
        }

        public async Task RemoveInstructorFromCenterAsync(
            Guid centerId,
            Guid instructorId,
            CancellationToken cancellationToken = default)
        {
            var membership = await _context.CenterInstructors
                .FirstOrDefaultAsync(ci =>
                    ci.CenterId == centerId &&
                    ci.InstructorId == instructorId,
                    cancellationToken);

            if (membership is not null)
            {
                membership.IsDeleted = true;
                _context.CenterInstructors.Update(membership);
            }

            var yearAssignments = await _context.CenterInstructorEducationYears
                .Where(y => y.CenterId == centerId && y.InstructorId == instructorId && y.IsActive)
                .ToListAsync(cancellationToken);

            foreach (var year in yearAssignments)
                year.IsActive = false;

            if (yearAssignments.Count > 0)
                _context.CenterInstructorEducationYears.UpdateRange(yearAssignments);
        }

        public async Task<bool> InstructorExistsAsync(Guid instructorUserId, CancellationToken cancellationToken = default)
        {
            return await _context.Instructors
                .AnyAsync(i => i.UserId == instructorUserId, cancellationToken);
        }
    }
}
