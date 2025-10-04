using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Student
    {
        public Guid UserId { get; set; }
        public string? DeviceId { get; set; }
        public int ScreenshotTrial { get; set; }

        public User User { get; set; } = null!;

        public ICollection<StudentCourse> StudentCourses { get; set; } = [];
        public ICollection<StudentSection> StudentSections { get; set; } = [];
        public ICollection<StudentVideo> StudentVideos { get; set; } = [];
        public ICollection<StudentSheet> StudentSheets { get; set; } = [];
        public ICollection<StudentExam> StudentExams { get; set; } = [];
        public ICollection<CourseReview> CourseReviews { get; set; } = [];
        public ICollection<SectionReview> SectionReviews { get; set; } = [];
        public ICollection<VideoReview> VideoReviews { get; set; } = [];
        public ICollection<ExamResult> ExamResults { get; set; } = [];
        public ICollection<Payment> Payments { get; set; } = [];
        public ICollection<StudentSubmission> StudentSubmissions { get; set; } = [];
    }
}


