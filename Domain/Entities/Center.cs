using Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("centers", Schema = "public")]
    public class Center : ISoftDeletableEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("logo_url")]
        public string? LogoUrl { get; set; }

        [Column("location_maps")]
        public string? LocationMaps { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; } = EgyptTime.UtcNow;

        [Column("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        // Navigation
        public ICollection<CenterInstructor> CenterInstructors { get; set; } = [];
        public ICollection<CenterInstructorEducationYear> CenterInstructorYears { get; set; } = [];
        public ICollection<Student> Students { get; set; } = [];
        public ICollection<CenterAdmin> CenterAdmins { get; set; } = [];
    }
}
