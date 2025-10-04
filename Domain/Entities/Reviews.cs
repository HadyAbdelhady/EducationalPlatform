using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class CourseReview
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public int StarRating { get; set; }
        public string? Comment { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public Student Student { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }

    public class SectionReview
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid SectionId { get; set; }
        public int StarRating { get; set; }
        public string? Comment { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public Student Student { get; set; } = null!;
        public Section Section { get; set; } = null!;
    }

    public class VideoReview
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid VideoId { get; set; }
        public int StarRating { get; set; }
        public string? Comment { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public Student Student { get; set; } = null!;
        public Video Video { get; set; } = null!;
    }
}


