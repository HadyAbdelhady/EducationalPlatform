using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("students", Schema = "public")]
    public class Student
    {
        [Key]
        [Column("user_id")]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [Column("device_id")]
        public string? DeviceId { get; set; }

        [Column("tried_screenshot")]
        public bool TriedScreenshot { get; set; }

        [Required]
        [Column("parent_phone_number")]
        public string ParentPhoneNumber { get; set; } = string.Empty;

        [Required]
        [Column("education_year_id")]
        [ForeignKey(nameof(EducationYear))]
        public Guid EducationYearId { get; set; }

        public User User { get; set; } = null!;

        public EducationYear EducationYear { get; set; } = null!;
        public ICollection<StudentCourse> StudentCourses { get; set; } = [];
        public ICollection<StudentSection> StudentSections { get; set; } = [];
        public ICollection<StudentVideo> StudentVideos { get; set; } = [];
        public ICollection<AnswersSheet> AnswersSheets { get; set; } = [];
        public ICollection<StudentExam> StudentExams { get; set; } = [];
        public ICollection<CourseReview> CourseReviews { get; set; } = [];
        public ICollection<SectionReview> SectionReviews { get; set; } = [];
        public ICollection<VideoReview> VideoReviews { get; set; } = [];
        public ICollection<InstructorReview> InstructorReviews { get; set; } = [];
        public ICollection<StudentExamResult> ExamResults { get; set; } = [];
        public ICollection<Payment> Payments { get; set; } = [];
        public ICollection<StudentAnswers> StudentSubmissions { get; set; } = [];
    }
}


