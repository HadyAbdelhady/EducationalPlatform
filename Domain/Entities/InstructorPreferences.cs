using Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("instructor_preferences")]
    public class InstructorPreferences : ISoftDeletableEntity
    {
        [Column("is_deleted")]
        public bool IsDeleted { get; set; }
        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("instructor_id")]
        [ForeignKey(nameof(Instructor))]

        public Guid InstructorId { get; set; }
        public Instructor Instructor { get; set; } = null!;
        [Column("application_name")]
        public string ApplicationName { get; set; } = string.Empty;
    }
}
