using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    [Table("sections", Schema = "public")]
    public class Section : ISoftDeletableEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Column("price")]
        public decimal Price { get; set; }

        [Column("number_of_videos")]
        public int NumberOfVideos { get; set; }

        [Column("number_of_exams")]
        public int NumberOfExams { get; set; }

        [Column("number_of_question_sheets")]
        public int NumberOfQuestionSheets { get; set; }

        [Column("rating")]
        public int Rating { get; set; } = 1;

        [Column("course_id")]
        [ForeignKey(nameof(Course))]
        public Guid? CourseId { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        public Course? Course { get; set; }

        public ICollection<Video> Videos { get; set; } = [];
        public ICollection<Sheet> Sheets { get; set; } = [];
        //public ICollection<QuestionsSheet> QuestionsSheets { get; set; } = [];
        public ICollection<InstructorSection> InstructorSections { get; set; } = [];
        public ICollection<StudentSection> StudentSections { get; set; } = [];
        public ICollection<SectionReview> SectionReviews { get; set; } = [];
        public ICollection<Exam> Exams { get; set; } = [];
        public ICollection<Payment> Payments { get; set; } = [];
    }
}


