using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    [Table("courses", Schema = "public")]
    public class Course : ISoftDeletableEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("price")]
        public decimal? Price { get; set; }

        [Column("picture_url")]
        public string? PictureUrl { get; set; }

        [Column("intro_video_url")]
        public string? IntroVideoUrl { get; set; }

        [Column("number_of_videos")]
        public int NumberOfVideos { get; set; } = 0;

        [Column("number_of_sections")]
        public int NumberOfSections { get; set; } = 0;

        [Column("number_of_exams")]
        public int NumberOfExams { get; set; } = 0;

        [Column("number_of_question_sheets")]
        public int NumberOfQuestionSheets { get; set; } = 0;

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        public ICollection<Section> Sections { get; set; } = [];
        public ICollection<InstructorCourse> InstructorCourses { get; set; } = [];
        public ICollection<StudentCourse> StudentCourses { get; set; } = [];
        public ICollection<CourseReview> CourseReviews { get; set; } = [];
        public ICollection<Sheet> Sheets { get; set; } = [];
        public ICollection<Exam> Exams { get; set; } = [];
        public ICollection<Payment> Payments { get; set; } = [];
    }
}


