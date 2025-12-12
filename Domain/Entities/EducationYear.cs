using Domain.Interfaces;

namespace Domain.Entities
{
    public class EducationYear : ISoftDeletableEntity
    {
        public Guid Id { get; set; }

        public string EducationYearName { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        // Navigation
        public ICollection<Student> Students { get; set; } = [];
    }
}
