using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public abstract class Review : ISoftDeletableEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("student_id")]
        [ForeignKey(nameof(Student))]
        public Guid StudentId { get; set; }

        public Guid EntityId { get; set; }

        [Column("star_rating")]
        public int StarRating { get; set; } = 1;

        [Column("comment")]
        public string Comment { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        [Column("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        public Student Student { get; set; } = null!;
    }

    [Table("course_reviews", Schema = "public")]
    public class CourseReview : Review
    {
        public Course Course { get; set; } = null!;
    }

    [Table("section_reviews", Schema = "public")]
    public class SectionReview : Review
    {
        public Section Section { get; set; } = null!;
    }

    [Table("video_reviews", Schema = "public")]
    public class VideoReview : Review
    {
        public Video Video { get; set; } = null!;

    }

    [Table("instructor_reviews", Schema = "public")]
    public class InstructorReview : Review
    {
        public Instructor Instructor { get; set; } = null!;
    }
}


