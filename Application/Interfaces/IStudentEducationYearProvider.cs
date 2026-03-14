namespace Application.Interfaces
{
    /// <summary>
    /// Provides education year context for students. Used to filter course lists by student's education year.
    /// </summary>
    public interface IStudentEducationYearProvider
    {
        /// <summary>
        /// Gets the education year ID for a user if they are a student.
        /// </summary>
        /// <param name="userId">The user ID (same as Student.UserId for students).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The education year ID if the user is a student, otherwise null.</returns>
        Task<Guid?> GetEducationYearIdByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
