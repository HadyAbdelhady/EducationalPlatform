using Application.DTOs.Center;

namespace Application.Interfaces
{
    public interface ICenterRepository
    {
        /// <summary>
        /// Returns the full center detail including nested instructor + education year info.
        /// </summary>
        Task<CenterResponse?> GetCenterWithDetailsAsync(Guid centerId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a lightweight list of all active centers.
        /// </summary>
        Task<IEnumerable<CenterSummaryResponse>> GetAllCentersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the InstructorIds that belong to a center.
        /// </summary>
        Task<IEnumerable<Guid>> GetInstructorIdsByCenterIdAsync(Guid centerId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks whether an instructor is an active member of a center.
        /// </summary>
        Task<bool> IsInstructorInCenterAsync(Guid centerId, Guid instructorId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the center ID of a given student (null if in instructor-only mode).
        /// </summary>
        Task<Guid?> GetStudentCenterIdAsync(Guid studentUserId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Atomically adds an instructor to a center with their education year assignments.
        /// </summary>
        Task AddInstructorToCenterAsync(
            Guid centerId,
            Guid instructorId,
            IEnumerable<Guid> educationYearIds,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Soft-removes an instructor from a center and deactivates all their year assignments within it.
        /// </summary>
        Task RemoveInstructorFromCenterAsync(
            Guid centerId,
            Guid instructorId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks whether a given instructor (by UserId) exists and is not deleted.
        /// </summary>
        Task<bool> InstructorExistsAsync(Guid instructorUserId, CancellationToken cancellationToken = default);
    }
}
