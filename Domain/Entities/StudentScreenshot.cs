using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("student_screenshots", Schema = "public")]
    public class StudentScreenshot
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("student_id")]
        [ForeignKey(nameof(Student))]
        public Guid StudentId { get; set; }

        [Column("entity_type")]
        [MaxLength(100)]
        public string? EntityType { get; set; }

        [Column("entity_id")]
        public Guid? EntityId { get; set; }

        [Column("image_url")]
        public string? ImageUrl { get; set; }

        [Column("page_name")]
        [MaxLength(255)]
        public string? PageName { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        public Student Student { get; set; } = null!;
    }
}
