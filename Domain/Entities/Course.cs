using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset DateOfCreation { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public decimal? Price { get; set; }
        public string? PictureUrl { get; set; }
        public string? IntroVideoUrl { get; set; }
        public int NumberOfVideos { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<Section> Sections { get; set; } = new List<Section>();
        public ICollection<InstructorCourse> InstructorCourses { get; set; } = new List<InstructorCourse>();
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
        public ICollection<CourseReview> CourseReviews { get; set; } = new List<CourseReview>();
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}


