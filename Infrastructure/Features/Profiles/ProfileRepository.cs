using Application.Features.Profiles.DTOs;
using Application.Features.Profiles.Interfaces;
using Infrastructure.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.Profiles
{
    public class ProfileRepository(EducationDbContext context) : IProfileRepository
    {
        private readonly EducationDbContext _context = context;

        public Task<bool> StudentExistsAsync(Guid studentId, CancellationToken cancellationToken = default) =>
            // Users global soft-delete filter excludes deleted users even if Student row remains
            _context.Students.AsNoTracking()
                .AnyAsync(s => s.UserId == studentId && s.User != null && s.EducationYear != null, cancellationToken);

        public Task<bool> InstructorExistsAsync(Guid instructorId, CancellationToken cancellationToken = default) =>
            _context.Instructors.AsNoTracking()
                .AnyAsync(i => i.UserId == instructorId && i.User != null, cancellationToken);

        public async Task<bool> HasSharedContentAsync(
            Guid instructorId,
            Guid studentId,
            CancellationToken cancellationToken = default)
        {
            // Course↔course or course-student↔section-instructor (same course)
            var inCourse = await _context.StudentCourses
                .AsNoTracking()
                .AnyAsync(
                    sc => sc.StudentId == studentId &&
                          (_context.InstructorCourses.Any(ic =>
                               ic.InstructorId == instructorId &&
                               ic.CourseId == sc.CourseId) ||
                           _context.InstructorSections.Any(ins =>
                               ins.InstructorId == instructorId &&
                               ins.Section.CourseId == sc.CourseId)),
                    cancellationToken);

            if (inCourse)
                return true;

            // Section↔section or section-student↔course-instructor (section in taught course)
            return await _context.StudentSections
                .AsNoTracking()
                .AnyAsync(
                    ss => ss.StudentId == studentId &&
                          (_context.InstructorSections.Any(ins =>
                               ins.InstructorId == instructorId &&
                               ins.SectionId == ss.SectionId) ||
                           _context.InstructorCourses.Any(ic =>
                               ic.InstructorId == instructorId &&
                               ic.CourseId == ss.Section.CourseId)),
                    cancellationToken);
        }

        public async Task<StudentProfileForInstructorResponse?> GetStudentProfileForInstructorAsync(
            Guid instructorId,
            Guid studentId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .AsNoTracking()
                .Where(s => s.UserId == studentId)
                .Select(s => new StudentProfileForInstructorResponse
                {
                    StudentId = s.UserId,
                    FullName = s.User.FullName,
                    Email = s.User.GmailExternal,
                    PersonalPictureUrl = s.User.PersonalPictureUrl,
                    EducationYearId = s.EducationYearId,
                    EducationYearName = s.EducationYear.EducationYearName,
                    CenterId = s.CenterId,
                    CenterName = s.Center != null ? s.Center.Name : null
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<InstructorProfileForStudentResponse?> GetInstructorProfileForStudentAsync(
            Guid studentId,
            Guid instructorId,
            CancellationToken cancellationToken = default)
        {
            var instructor = await _context.Instructors
                .AsNoTracking()
                .Where(i => i.UserId == instructorId)
                .Select(i => new InstructorProfileForStudentResponse
                {
                    InstructorId = i.UserId,
                    FullName = i.User.FullName,
                    PersonalPictureUrl = i.User.PersonalPictureUrl,
                    GmailExternal = i.User.GmailExternal,
                    Rating = i.Rating
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (instructor is null)
                return null;

            instructor.SharedContent = await LoadSharedEnrollmentsAsync(
                instructorId,
                studentId,
                cancellationToken);

            return instructor;
        }

        private async Task<List<SharedEnrollmentDto>> LoadSharedEnrollmentsAsync(
            Guid instructorId,
            Guid studentId,
            CancellationToken cancellationToken)
        {
            var courses = await _context.InstructorCourses
                .AsNoTracking()
                .Where(ic => ic.InstructorId == instructorId)
                .Select(ic => new SharedEnrollmentDto
                {
                    CourseId = ic.CourseId,
                    CourseName = ic.Course.Name,
                    Description = ic.Course.Description,
                    PictureUrl = ic.Course.PictureUrl,
                    Price = ic.Course.Price,
                    NumberOfVideos = ic.Course.NumberOfVideos,
                    NumberOfSections = ic.Course.NumberOfSections,
                    NumberOfSheets = ic.Course.NumberOfQuestionSheets,
                    NumberOfExams = ic.Course.NumberOfExams,
                    NumberOfStudents = ic.Course.NumberOfStudentsEnrolled,
                    Rating = ic.Course.Rating,
                    IsEnrolled = ic.Course.StudentCourses.Any(sc => sc.StudentId == studentId),
                    EnrolledAt = ic.Course.StudentCourses
                        .Where(sc => sc.StudentId == studentId)
                        .Select(sc => (DateTimeOffset?)sc.EnrolledAt)
                        .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);

            var taughtCourseIds = courses
                .Where(c => c.CourseId.HasValue)
                .Select(c => c.CourseId!.Value)
                .ToHashSet();

            var sections = await _context.InstructorSections
                .AsNoTracking()
                .Where(ins => ins.InstructorId == instructorId &&
                              !taughtCourseIds.Contains(ins.Section.CourseId))
                .Select(ins => new SharedEnrollmentDto
                {
                    CourseId = ins.Section.CourseId,
                    CourseName = ins.Section.Course!.Name,
                    SectionId = ins.SectionId,
                    SectionName = ins.Section.Name,
                    Description = ins.Section.Description,
                    Price = ins.Section.Price,
                    NumberOfVideos = ins.Section.NumberOfVideos,
                    NumberOfSheets = ins.Section.NumberOfQuestionSheets,
                    NumberOfExams = ins.Section.NumberOfExams,
                    NumberOfStudents = ins.Section.NumberOfStudentsEnrolled,
                    Rating = ins.Section.Rating,
                    IsEnrolled = ins.Section.StudentSections.Any(ss => ss.StudentId == studentId) ||
                                 ins.Section.Course!.StudentCourses.Any(sc => sc.StudentId == studentId),
                    EnrolledAt = ins.Section.StudentSections
                        .Where(ss => ss.StudentId == studentId)
                        .Select(ss => (DateTimeOffset?)ss.EnrolledAt)
                        .FirstOrDefault()
                        ?? ins.Section.Course!.StudentCourses
                            .Where(sc => sc.StudentId == studentId)
                            .Select(sc => (DateTimeOffset?)sc.EnrolledAt)
                            .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);

            return courses
                .Concat(sections)
                .OrderByDescending(e => e.IsEnrolled)
                .ThenByDescending(e => e.EnrolledAt)
                .ThenBy(e => e.CourseName)
                .ToList();
        }
    }
}
