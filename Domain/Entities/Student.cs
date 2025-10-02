using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Student
    {
        public Guid UserId { get; set; }
        public string? DeviceId { get; set; }
        public int ScreenshotTrial { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public User User { get; set; } = null!;

        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
        public ICollection<StudentSection> StudentSections { get; set; } = new List<StudentSection>();
        public ICollection<StudentVideo> StudentVideos { get; set; } = new List<StudentVideo>();
        public ICollection<StudentSheet> StudentSheets { get; set; } = new List<StudentSheet>();
        public ICollection<StudentExam> StudentExams { get; set; } = new List<StudentExam>();
        public ICollection<CourseReview> CourseReviews { get; set; } = new List<CourseReview>();
        public ICollection<SectionReview> SectionReviews { get; set; } = new List<SectionReview>();
        public ICollection<VideoReview> VideoReviews { get; set; } = new List<VideoReview>();
        public ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<StudentSubmission> StudentSubmissions { get; set; } = new List<StudentSubmission>();
    }
}


