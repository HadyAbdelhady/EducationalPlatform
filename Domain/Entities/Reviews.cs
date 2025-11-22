using Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public abstract class Review : ISoftDeletableEntity
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid EntityId { get; set; }
        public int StarRating { get; set; }
        public string? Comment { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public Student Student { get; set; } = null!;
        public required string EntityType { get; set; }
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
    }
}


