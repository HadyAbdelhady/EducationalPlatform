using Application.Features.HomeScreen.DTOs;
using Application.Common.Interfaces;
using Application.Features.HomeScreen.Interfaces;
using Application.Common;
using Domain;
using Infrastructure.Common.Data;
using Infrastructure.Features.HomeScreen.EnrollmentProgress;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.HomeScreen
{
    public class EnrollmentProgressRepository(
        EducationDbContext context,
        EnrollmentProgressHelper progressHelper) : IEnrollmentProgressRepository
    {
        private readonly EducationDbContext _context = context;
        private readonly EnrollmentProgressHelper _progressHelper = progressHelper;

        public async Task<StudentEnrollmentProgressResponse> GetStudentEnrollmentProgressAsync(
            Guid studentId,
            int enrollmentsPage,
            int enrollmentsPageSize,
            int milestonesPage,
            int milestonesPageSize,
            CancellationToken cancellationToken = default)
        {
            enrollmentsPage = enrollmentsPage < 1 ? 1 : enrollmentsPage;
            enrollmentsPageSize = enrollmentsPageSize <= 0 ? 6 : enrollmentsPageSize;
            milestonesPage = milestonesPage < 1 ? 1 : milestonesPage;
            milestonesPageSize = milestonesPageSize <= 0 ? 10 : milestonesPageSize;

            var studentCoursesAll = await _context.StudentCourses
                .AsNoTracking()
                .Where(sc => sc.StudentId == studentId)
                .Select(sc => new
                {
                    sc.CourseId,
                    sc.NumberOfCourseVideosWatched,
                    CourseNumberOfVideos = sc.Course.NumberOfVideos
                })
                .ToListAsync(cancellationToken);

            var sectionOnlyWatched = await _context.StudentSections
                .AsNoTracking()
                .Where(ss => ss.StudentId == studentId)
                .Where(ss => !_context.StudentCourses.Any(sc =>
                    sc.StudentId == studentId && sc.CourseId == ss.Section.CourseId))
                .Select(ss => new
                {
                    ss.NumberOfSectionVideosWatched,
                    SectionNumberOfVideos = ss.Section.NumberOfVideos
                })
                .ToListAsync(cancellationToken);

            var inProgressCoursesCount = studentCoursesAll
                .Count(sc =>
                    sc.CourseNumberOfVideos > 0 &&
                    sc.NumberOfCourseVideosWatched < sc.CourseNumberOfVideos)
                + sectionOnlyWatched
                .Count(ss =>
                    ss.SectionNumberOfVideos > 0 &&
                    ss.NumberOfSectionVideosWatched < ss.SectionNumberOfVideos);

            var completedLessonsCount = studentCoursesAll.Sum(sc => sc.NumberOfCourseVideosWatched)
                + sectionOnlyWatched.Sum(ss => ss.NumberOfSectionVideosWatched);

            var examResults = await _context.ExamResults
                .AsNoTracking()
                .Where(er =>
                    er.StudentId == studentId &&
                    er.StudentMark.HasValue &&
                    er.Exam.TotalMark > 0)
                .Select(er => new { er.StudentMark, er.Exam.TotalMark })
                .ToListAsync(cancellationToken);

            var averageGrade = examResults.Count > 0
                ? examResults.Average(er => (er.StudentMark!.Value / er.TotalMark) * 100m)
                : 0m;

            var scope = EnrollmentProgressScope.Unrestricted;
            var enrollmentsTotalCount = await _progressHelper.CountEnrollmentStubsForStudentAsync(
                studentId,
                scope,
                cancellationToken);

            var pageEnrollments = await _progressHelper.GetEnrollmentStubPageForStudentAsync(
                studentId,
                scope,
                enrollmentsPage,
                enrollmentsPageSize,
                cancellationToken);

            var enrollmentDtos = await _progressHelper.BuildEnrollmentProgressDtosAsync(
                studentId,
                pageEnrollments,
                scope,
                cancellationToken);

            var paginatedMilestones = await BuildUpcomingMilestonesAsync(
                studentId,
                milestonesPage,
                milestonesPageSize,
                cancellationToken);

            return new StudentEnrollmentProgressResponse
            {
                Global = new ProgressGlobalSummary
                {
                    InProgressCoursesCount = inProgressCoursesCount,
                    CompletedLessonsCount = completedLessonsCount,
                    AverageGrade = averageGrade
                },
                Enrollments = new PaginatedResult<EnrollmentProgressDto>
                {
                    Items = enrollmentDtos,
                    PageNumber = enrollmentsPage,
                    PageSize = enrollmentsPageSize,
                    TotalCount = enrollmentsTotalCount
                },
                Milestones = paginatedMilestones
            };
        }

        public async Task<InstructorStudentsProgressResponse> GetInstructorStudentsProgressAsync(
            Guid instructorId,
            HashSet<Guid> allowedCourseIds,
            HashSet<Guid> allowedSectionIds,
            Guid? studentId,
            int page,
            int pageSize,
            string? search,
            Guid? educationYearId,
            CancellationToken cancellationToken = default)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, EnrollmentProgressHelper.MaxInstructorPageSize);

            if (allowedCourseIds.Count == 0 && allowedSectionIds.Count == 0)
            {
                return new InstructorStudentsProgressResponse
                {
                    Students = QueryableFilterExtensions.EmptyPaginatedResult<InstructorStudentProgressDto>(page, pageSize)
                };
            }

            var scope = EnrollmentProgressScope.ForInstructor(allowedCourseIds, allowedSectionIds);

            var (totalCount, pageSummaries) = await _progressHelper.GetInstructorStudentPageAsync(
                allowedCourseIds,
                allowedSectionIds,
                studentId,
                page,
                pageSize,
                search,
                educationYearId,
                cancellationToken);

            var pageStudentIds = pageSummaries.Select(s => s.StudentId).ToList();
            var progressByStudent = await _progressHelper.BuildEnrollmentProgressForStudentsAsync(
                pageStudentIds,
                scope,
                cancellationToken);

            var studentDtos = pageSummaries
                .Select(summary =>
                {
                    var enrollments = progressByStudent.TryGetValue(summary.StudentId, out var items)
                        ? items
                        : [];

                    return new InstructorStudentProgressDto
                    {
                        StudentId = summary.StudentId,
                        StudentName = summary.StudentName,
                        StudentEmail = summary.StudentEmail,
                        StudentPictureUrl = summary.StudentPictureUrl,
                        PhoneNumber = summary.PhoneNumber,
                        ParentPhoneNumber = summary.ParentPhoneNumber,
                        LocationMaps = summary.LocationMaps,
                        Gender = summary.Gender,
                        EducationYearId = summary.EducationYearId,
                        Overall = EnrollmentProgressMapper.AggregateOverallProgress(enrollments)
                    };
                })
                .ToList();

            return new InstructorStudentsProgressResponse
            {
                Students = new PaginatedResult<InstructorStudentProgressDto>
                {
                    Items = studentDtos,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                }
            };
        }

        public async Task<InstructorStudentEnrollmentsResponse> GetInstructorStudentEnrollmentsAsync(
            Guid instructorId,
            Guid studentId,
            HashSet<Guid> allowedCourseIds,
            HashSet<Guid> allowedSectionIds,
            CancellationToken cancellationToken = default)
        {
            var scope = EnrollmentProgressScope.ForInstructor(allowedCourseIds, allowedSectionIds);
            var progressByStudent = await _progressHelper.BuildEnrollmentProgressForStudentsAsync(
                [studentId],
                scope,
                cancellationToken);

            var enrollments = progressByStudent.TryGetValue(studentId, out var items)
                ? items
                : [];

            var instructorName = await _context.Instructors
                .AsNoTracking()
                .Where(i => i.UserId == instructorId)
                .Select(i => i.User.FullName)
                .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

            var instructorReviews = await _context.InstructorReviews
                .AsNoTracking()
                .Where(r => r.StudentId == studentId && r.EntityId == instructorId)
                .Select(r => new StudentContentReviewDto
                {
                    Id = r.Id,
                    Type = "Instructor",
                    EntityId = r.EntityId,
                    EntityName = instructorName,
                    StarRating = r.StarRating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync(cancellationToken);

            var courseReviews = allowedCourseIds.Count == 0
                ? []
                : await _context.CourseReviews
                    .AsNoTracking()
                    .Where(r => r.StudentId == studentId && allowedCourseIds.Contains(r.EntityId))
                    .Select(r => new StudentContentReviewDto
                    {
                        Id = r.Id,
                        Type = "Course",
                        EntityId = r.EntityId,
                        EntityName = r.Course.Name,
                        StarRating = r.StarRating,
                        Comment = r.Comment,
                        CreatedAt = r.CreatedAt
                    })
                    .ToListAsync(cancellationToken);

            var sectionReviews = allowedSectionIds.Count == 0
                ? []
                : await _context.SectionReviews
                    .AsNoTracking()
                    .Where(r => r.StudentId == studentId && allowedSectionIds.Contains(r.EntityId))
                    .Select(r => new StudentContentReviewDto
                    {
                        Id = r.Id,
                        Type = "Section",
                        EntityId = r.EntityId,
                        EntityName = r.Section.Name,
                        StarRating = r.StarRating,
                        Comment = r.Comment,
                        CreatedAt = r.CreatedAt
                    })
                    .ToListAsync(cancellationToken);

            return new InstructorStudentEnrollmentsResponse
            {
                StudentId = studentId,
                Enrollments = enrollments,
                Reviews = instructorReviews
                    .Concat(courseReviews)
                    .Concat(sectionReviews)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList()
            };
        }

        private async Task<PaginatedResult<UpcomingMilestoneDto>> BuildUpcomingMilestonesAsync(
            Guid studentId,
            int milestonesPage,
            int milestonesPageSize,
            CancellationToken cancellationToken)
        {
            var now = EgyptTime.UtcNow;

            var upcomingExamsQuery = _context.Exams
                .AsNoTracking()
                .Where(e =>
                    e.StartTime.HasValue &&
                    e.StartTime.Value >= now &&
                    _context.StudentCourses.Any(sc =>
                        sc.StudentId == studentId &&
                        sc.CourseId == e.CourseId))
                .Select(e => new UpcomingMilestoneDto
                {
                    Id = e.Id,
                    Title = e.Name,
                    CourseName = e.Course != null ? e.Course.Name : string.Empty,
                    Type = "Exam",
                    DueAt = e.StartTime!.Value
                });

            var upcomingSheetsQuery = _context.Sheets
                .AsNoTracking()
                .Where(s =>
                    s.CourseId.HasValue &&
                    s.DueDate.HasValue &&
                    s.DueDate.Value >= now &&
                    _context.StudentCourses.Any(sc =>
                        sc.StudentId == studentId &&
                        sc.CourseId == s.CourseId!.Value))
                .Select(s => new UpcomingMilestoneDto
                {
                    Id = s.Id,
                    Title = s.Name,
                    CourseName = s.Course != null ? s.Course.Name : string.Empty,
                    Type = "Sheet",
                    DueAt = s.DueDate!.Value
                });

            var milestonesQuery = upcomingExamsQuery.Concat(upcomingSheetsQuery);
            return await milestonesQuery
                .OrderBy(m => m.DueAt)
                .ToPaginatedResultAsync(milestonesPage, milestonesPageSize, cancellationToken);
        }
    }
}
