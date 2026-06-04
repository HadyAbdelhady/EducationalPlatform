using Application.DTOs.HomeScreen;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain;
using Infrastructure.Data;
using Infrastructure.Progress;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
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

            var inProgressCoursesCount = studentCoursesAll
                .Count(sc =>
                    sc.CourseNumberOfVideos > 0 &&
                    sc.NumberOfCourseVideosWatched < sc.CourseNumberOfVideos);

            var completedLessonsCount = studentCoursesAll.Sum(sc => sc.NumberOfCourseVideosWatched);

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
            CancellationToken cancellationToken = default)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, EnrollmentProgressHelper.MaxInstructorPageSize);

            if (allowedCourseIds.Count == 0 && allowedSectionIds.Count == 0)
            {
                return new InstructorStudentsProgressResponse
                {
                    Students = new PaginatedResult<InstructorStudentProgressDto>
                    {
                        Items = [],
                        PageNumber = page,
                        PageSize = pageSize,
                        TotalCount = 0
                    }
                };
            }

            var scope = EnrollmentProgressScope.ForInstructor(allowedCourseIds, allowedSectionIds);

            var (totalCount, pageSummaries) = await _progressHelper.GetInstructorStudentPageAsync(
                allowedCourseIds,
                allowedSectionIds,
                studentId,
                page,
                pageSize,
                cancellationToken);

            var pageStudentIds = pageSummaries.Select(s => s.StudentId).ToList();
            var progressByStudent = await _progressHelper.BuildEnrollmentProgressForStudentsAsync(
                pageStudentIds,
                scope,
                cancellationToken);

            var studentDtos = pageSummaries
                .Select(summary => new InstructorStudentProgressDto
                {
                    StudentId = summary.StudentId,
                    StudentName = summary.StudentName,
                    StudentEmail = summary.StudentEmail,
                    Enrollments = progressByStudent.TryGetValue(summary.StudentId, out var enrollments)
                        ? enrollments
                        : []
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
            var milestonesTotalCount = await milestonesQuery.CountAsync(cancellationToken);
            var milestonesItems = await milestonesQuery
                .OrderBy(m => m.DueAt)
                .Skip((milestonesPage - 1) * milestonesPageSize)
                .Take(milestonesPageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedResult<UpcomingMilestoneDto>
            {
                Items = milestonesItems,
                PageNumber = milestonesPage,
                PageSize = milestonesPageSize,
                TotalCount = milestonesTotalCount
            };
        }
    }
}
