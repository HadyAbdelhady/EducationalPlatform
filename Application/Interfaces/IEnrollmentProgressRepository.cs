using Application.DTOs.HomeScreen;

namespace Application.Interfaces
{
    public interface IEnrollmentProgressRepository
    {
        Task<StudentEnrollmentProgressResponse> GetStudentEnrollmentProgressAsync(
            Guid studentId,
            int enrollmentsPage,
            int enrollmentsPageSize,
            int milestonesPage,
            int milestonesPageSize,
            CancellationToken cancellationToken = default);

        Task<InstructorStudentsProgressResponse> GetInstructorStudentsProgressAsync(
            Guid instructorId,
            HashSet<Guid> allowedCourseIds,
            HashSet<Guid> allowedSectionIds,
            Guid? studentId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}
