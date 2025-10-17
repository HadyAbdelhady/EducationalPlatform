using Domain.Interfaces;

namespace Domain.Entities
{
    public class User : ISoftDeletableEntity
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Ssn { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? GmailExternal { get; set; }
        public string? AppleExternal { get; set; }
        public string? PersonalPictureUrl { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string EducationYear { get; set; } = string.Empty;
        public string? LocationMaps { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public Student? Student { get; set; }
        public Instructor? Instructor { get; set; }
    }
}


