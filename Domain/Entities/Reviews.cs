using Domain.Interfaces;

namespace Domain.Entities
{
    public abstract class Review : ISoftDeletableEntity
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid EntityId { get; set; }
        public int StarRating { get; set; } = 1;
        public string Comment { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        public Student Student { get; set; } = null!;
    }

    public class CourseReview : Review
    {
        public Course Course { get; set; } = null!;
    }

    public class SectionReview : Review
    {
        public Section Section { get; set; } = null!;
    }

    public class VideoReview : Review
    {
        public Video Video { get; set; } = null!;

        public VideoReview()
        {

        }
    }

    public class InstructorReview : Review
    {
        public Instructor Instructor { get; set; } = null!;
    }
}


