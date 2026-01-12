using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    [Table("questions", Schema = "public")]
    public class Question : ISoftDeletableEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("section_id")]
        [ForeignKey(nameof(Section))]
        public Guid SectionId { get; set; }

        [Required]
        [Column("course_id")]
        [ForeignKey(nameof(Course))]
        public Guid CourseId { get; set; }

        [Required]
        [Column("question_string")]
        public string QuestionString { get; set; } = string.Empty;

        [Column("question_image_url")]
        public string? QuestionImageUrl { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        [Column("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        public Section? Section { get; set; } = null;
        public Course? Course { get; set; } = null;
        public ICollection<Answer> Answers { get; set; } = [];
        public ICollection<StudentAnswers> StudentSubmissions { get; set; } = [];
    }
}


