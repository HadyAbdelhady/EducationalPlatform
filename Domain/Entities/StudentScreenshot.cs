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

        [Required]
        [Column("image_url")]
        public string ImageUrl { get; set; } = string.Empty;

        [Column("page_name")]
        [MaxLength(255)]
        public string? PageName { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        public Student Student { get; set; } = null!;
    }
}
