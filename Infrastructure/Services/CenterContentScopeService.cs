using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    /// <summary>
    /// Enforces the Center-mode purchase authorization rule:
    /// a student may only buy courses/sections from instructors within their assigned center.
    /// When the student has no center assigned (Student.CenterId == null),
    /// this service is a no-op — supporting instructor-only mode deployments transparently.
    /// </summary>
    public class CenterContentScopeService(EducationDbContext context) : ICenterContentScopeService
    {
        private readonly EducationDbContext _context = context;

        public async Task ValidatePurchaseAsync(
            Guid studentUserId,
            Guid? courseId,
            Guid? sectionId,
            CancellationToken cancellationToken = default)
        {
            // ── 1. Get the student's center (null = instructor-only mode) ──────────
            var centerId = await _context.Students
                .AsNoTracking()
                .Where(s => s.UserId == studentUserId)
                .Select(s => s.CenterId)
                .FirstOrDefaultAsync(cancellationToken);

            if (centerId is null)
                return; // Instructor-only mode — no restriction applies

            // ── 2. Resolve the instructor of the content being purchased ───────────
            Guid? instructorId = null;

            if (courseId.HasValue)
            {
                instructorId = await _context.InstructorCourses
                    .AsNoTracking()
                    .Where(ic => ic.CourseId == courseId.Value && !ic.IsDeleted)
                    .Select(ic => (Guid?)ic.InstructorId)
                    .FirstOrDefaultAsync(cancellationToken);
            }
            else if (sectionId.HasValue)
            {
                instructorId = await _context.InstructorSections
                    .AsNoTracking()
                    .Where(ins => ins.SectionId == sectionId.Value && !ins.IsDeleted)
                    .Select(ins => (Guid?)ins.InstructorId)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            if (instructorId is null)
                return; // Content has no assigned instructor — allow (edge case)

            // ── 3. Assert the instructor belongs to the student's center ───────────
            var belongs = await _context.CenterInstructors
                .AnyAsync(ci => ci.CenterId == centerId.Value &&
                                ci.InstructorId == instructorId.Value &&
                                !ci.IsDeleted,
                          cancellationToken);

            if (!belongs)
                throw new UnauthorizedAccessException(
                    "You can only purchase content from instructors within your assigned Center.");
        }
    }
}
