namespace Application.Interfaces
{
    /// <summary>
    /// Authorization guard for Center-mode deployments.
    /// Validates that a student only purchases content from instructors inside their assigned center.
    /// When the student has no center (instructor-only mode), all checks are bypassed.
    /// </summary>
    public interface ICenterContentScopeService
    {
        /// <summary>
        /// Ensures the course or section being purchased belongs to an instructor
        /// that is a member of the student's center.
        /// Throws <see cref="UnauthorizedAccessException"/> if the restriction is violated.
        /// Does nothing if the student has no assigned center (instructor-only mode).
        /// </summary>
        Task ValidatePurchaseAsync(
            Guid studentUserId,
            Guid? courseId,
            Guid? sectionId,
            CancellationToken cancellationToken = default);
    }
}
