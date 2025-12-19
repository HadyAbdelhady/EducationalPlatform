using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        [Column("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        public Student Student { get; set; } = null!;
    }

    [Table("course_reviews", Schema = "public")]
    public class CourseReview : Review
    {
        [Column("course_id")]
        [ForeignKey(nameof(Course))]
        public Guid EntityId { get; set; }

        public Course Course { get; set; } = null!;
    }

    [Table("section_reviews", Schema = "public")]
    public class SectionReview : Review
    {
        [Column("section_id")]
        [ForeignKey(nameof(Section))]
        public Guid EntityId { get; set; }

        public Section Section { get; set; } = null!;
    }

    [Table("video_reviews", Schema = "public")]
    public class VideoReview : Review
    {
        [Column("video_id")]
        [ForeignKey(nameof(Video))]
        public Guid EntityId { get; set; }

        public Video Video { get; set; } = null!;

        public VideoReview()
        {

        }
    }

    [Table("instructor_reviews", Schema = "public")]
    public class InstructorReview : Review
    {
        [Column("instructor_id")]
        [ForeignKey(nameof(Instructor))]
        public Guid EntityId { get; set; }

        public Instructor Instructor { get; set; } = null!;
    }
}


