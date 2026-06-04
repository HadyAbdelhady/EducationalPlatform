namespace Infrastructure.Progress
{
    /// <summary>
    /// Limits which enrollments are included. When a set is null, no filter is applied for that dimension.
    /// When provided, only matching course/section enrollments are returned.
    /// </summary>
    public sealed class EnrollmentProgressScope
    {
        public HashSet<Guid>? AllowedCourseIds { get; init; }

        public HashSet<Guid>? AllowedSectionIds { get; init; }

        public static EnrollmentProgressScope Unrestricted => new();

        public static EnrollmentProgressScope ForInstructor(HashSet<Guid> courseIds, HashSet<Guid> sectionIds) =>
            new() { AllowedCourseIds = courseIds, AllowedSectionIds = sectionIds };
    }
}
