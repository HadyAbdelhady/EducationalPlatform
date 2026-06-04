using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class InstructorContentScopeService(EducationDbContext context) : IInstructorContentScopeService
    {
        private readonly EducationDbContext _context = context;

        public async Task<InstructorContentScope> ResolveAsync(
            Guid instructorId,
            Guid? courseId,
            Guid? sectionId,
            CancellationToken cancellationToken = default)
        {
            var instructorCourseIds = await _context.InstructorCourses
                .AsNoTracking()
                .Where(ic => ic.InstructorId == instructorId)
                .Select(ic => ic.CourseId)
                .ToListAsync(cancellationToken);

            var instructorSectionIds = await _context.InstructorSections
                .AsNoTracking()
                .Where(ins => ins.InstructorId == instructorId)
                .Select(ins => ins.SectionId)
                .ToListAsync(cancellationToken);

            var courseIdSet = instructorCourseIds.ToHashSet();
            var sectionIdSet = instructorSectionIds.ToHashSet();

            if (courseId.HasValue)
            {
                if (!courseIdSet.Contains(courseId.Value))
                    throw new UnauthorizedAccessException("You do not teach this course.");

                courseIdSet = [courseId.Value];
                sectionIdSet = await _context.InstructorSections
                    .AsNoTracking()
                    .Where(ins => ins.InstructorId == instructorId && ins.Section.CourseId == courseId.Value)
                    .Select(ins => ins.SectionId)
                    .ToHashSetAsync(cancellationToken);
            }

            if (sectionId.HasValue)
            {
                if (!instructorSectionIds.Contains(sectionId.Value))
                    throw new UnauthorizedAccessException("You do not teach this section.");

                sectionIdSet = [sectionId.Value];

                if (!courseId.HasValue)
                {
                    var parentCourseId = await _context.Sections
                        .AsNoTracking()
                        .Where(s => s.Id == sectionId.Value)
                        .Select(s => s.CourseId)
                        .FirstOrDefaultAsync(cancellationToken);

                    courseIdSet = parentCourseId != Guid.Empty && instructorCourseIds.Contains(parentCourseId)
                        ? [parentCourseId]
                        : [];
                }
            }

            return new InstructorContentScope
            {
                CourseIds = courseIdSet,
                SectionIds = sectionIdSet
            };
        }
    }
}
