using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.enums;
using Domain.Interfaces;

namespace Domain.Entities
{
    public abstract class SheetBase : ISoftDeletableEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("sheet_url")]
        public string SheetUrl { get; set; } = null!;

        [Required]
        [Column("sheet_public_id")]
        public string SheetPublicId { get; set; } = null!;

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; } 

        [Column("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }
    }

    [Table("answers_sheets", Schema = "public")]
    public class AnswersSheet : SheetBase
    {
        [Column("questions_sheets_id")]
        [ForeignKey(nameof(QuestionsSheet))]
        public Guid QuestionsSheetId { get; set; }

        [Column("student_id")]
        [ForeignKey(nameof(Student))]
        public Guid StudentId { get; set; }

        [Column("is_approved")]
        public bool IsApproved  { get; set; }

        public Sheet QuestionsSheet { get; set; } = null!;
        public Student Student { get; set; } = null!;
    }

    [Table("sheets", Schema = "public")]
    public class Sheet : SheetBase
    {
        [Column("section_id")]
        [ForeignKey(nameof(Section))]
        public Guid? SectionId { get; set; }

        [Column("course_id")]
        [ForeignKey(nameof(Course))]
        public Guid? CourseId { get; set; }

        [Column("video_id")]
        [ForeignKey(nameof(Video))]
        public Guid? VideoId { get; set; }

        [Column("instructor_id")]
        [ForeignKey(nameof(Instructor))]
        public Guid InstructorId { get; set; }

        [Column("type")]
        public SheetType Type { get; set; }

        [Column("due_date")]
        public DateTime? DueDate { get; set; }

        public Section? Section { get; set; }
        public Video? Video { get; set; }
        public Course? Course { get; set; }
        public Instructor Instructor { get; set; } = null!;
        public ICollection<AnswersSheet> AnswersSheets { get; set; } = [];
    }
}


