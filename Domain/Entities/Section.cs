using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Section
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset DateOfCreation { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public decimal? Price { get; set; }
        public int NumberOfVideos { get; set; }
        public decimal? Rating { get; set; }
        public Guid? CourseId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public Course? Course { get; set; }

        public ICollection<Video> Videos { get; set; } = new List<Video>();
        public ICollection<Sheet> Sheets { get; set; } = new List<Sheet>();
        public ICollection<InstructorSection> InstructorSections { get; set; } = new List<InstructorSection>();
        public ICollection<StudentSection> StudentSections { get; set; } = new List<StudentSection>();
        public ICollection<SectionReview> SectionReviews { get; set; } = new List<SectionReview>();
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}


