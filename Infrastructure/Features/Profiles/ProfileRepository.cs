using Application.Features.Profiles.DTOs;
using Application.Features.Profiles.Interfaces;
using Application.Features.Reviews.DTOs;
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

            // ponytail: cap 20 newest; empty list OK (don't use ReviewService NotFound-on-empty)
            instructor.Reviews = await _context.InstructorReviews
                .AsNoTracking()
                .Where(r => r.EntityId == instructorId)
                .OrderByDescending(r => r.CreatedAt)
                .Take(20)
                .Select(r => new GetAllReviewsResponse
                {
                    Id = r.Id,
                    StudentId = r.StudentId,
                    StarRating = r.StarRating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt ?? r.CreatedAt,
                    Student = r.Student != null && r.Student.User != null
                        ? new StudentReviewInfo
                        {
                            StudentId = r.StudentId,
                            FullName = r.Student.User.FullName,
                            PersonalPictureUrl = r.Student.User.PersonalPictureUrl
                        }
                        : null
                })
                .ToListAsync(cancellationToken);

            return instructor;
        }

        private async Task<List<SharedEnrollmentDto>> LoadSharedEnrollmentsAsync(
            Guid instructorId,
            Guid studentId,
            CancellationToken cancellationToken)
        {
            var taughtCourseIds = await _context.InstructorCourses
                .AsNoTracking()
                .Where(ic => ic.InstructorId == instructorId)
                .Select(ic => ic.CourseId)
                .ToListAsync(cancellationToken);

            var taughtSectionIds = await _context.InstructorSections
                .AsNoTracking()
                .Where(ins => ins.InstructorId == instructorId)
                .Select(ins => ins.SectionId)
                .ToListAsync(cancellationToken);

            var courseEnrollments = await _context.StudentCourses
                .AsNoTracking()
                .Where(sc => sc.StudentId == studentId &&
                             (taughtCourseIds.Contains(sc.CourseId) ||
                              _context.InstructorSections.Any(ins =>
                                  ins.InstructorId == instructorId &&
                                  ins.Section.CourseId == sc.CourseId)))
                .Select(sc => new SharedEnrollmentDto
                {
                    CourseId = sc.CourseId,
                    CourseName = sc.Course.Name,
                    EnrolledAt = sc.EnrolledAt,
                    Description = sc.Course.Description,
                    PictureUrl = sc.Course.PictureUrl,
                    Price = sc.Course.Price,
                    NumberOfVideos = sc.Course.NumberOfVideos,
                    NumberOfSections = sc.Course.NumberOfSections,
                    NumberOfSheets = sc.Course.NumberOfQuestionSheets,
                    NumberOfExams = sc.Course.NumberOfExams,
                    NumberOfStudents = sc.Course.NumberOfStudentsEnrolled,
                    Rating = sc.Course.Rating
                })
                .ToListAsync(cancellationToken);

            var enrolledCourseIds = courseEnrollments
                .Where(e => e.CourseId.HasValue)
                .Select(e => e.CourseId!.Value)
                .ToHashSet();

            var sectionEnrollments = await _context.StudentSections
                .AsNoTracking()
                .Where(ss => ss.StudentId == studentId &&
                             (taughtSectionIds.Contains(ss.SectionId) ||
                              taughtCourseIds.Contains(ss.Section.CourseId)))
                .Where(ss => !enrolledCourseIds.Contains(ss.Section.CourseId))
                .Select(ss => new SharedEnrollmentDto
                {
                    CourseId = ss.Section.CourseId,
                    CourseName = ss.Section.Course!.Name,
                    SectionId = ss.SectionId,
                    SectionName = ss.Section.Name,
                    EnrolledAt = ss.EnrolledAt,
                    Description = ss.Section.Description,
                    Price = ss.Section.Price,
                    NumberOfVideos = ss.Section.NumberOfVideos,
                    NumberOfSheets = ss.Section.NumberOfQuestionSheets,
                    NumberOfExams = ss.Section.NumberOfExams,
                    NumberOfStudents = ss.Section.StudentSections.Count(),
                    Rating = ss.Section.Rating
                })
                .ToListAsync(cancellationToken);

            return courseEnrollments
                .Concat(sectionEnrollments)
                .OrderByDescending(e => e.EnrolledAt)
                .ToList();
        }
    }
}
