namespace Application.Interfaces
{
    /// <summary>
    /// Resolves which course/section IDs an instructor may access for progress queries.
    /// </summary>
    public interface IInstructorContentScopeService
    {
        Task<InstructorContentScope> ResolveAsync(
            Guid instructorId,
            Guid? courseId,
            Guid? sectionId,
            CancellationToken cancellationToken = default);
    }

    public sealed class InstructorContentScope
    {
        public HashSet<Guid> CourseIds { get; init; } = [];

        public HashSet<Guid> SectionIds { get; init; } = [];
    }
}
