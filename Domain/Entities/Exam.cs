using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.enums;
using Domain.Interfaces;

namespace Domain.Entities
{
    [Table("exams", Schema = "public")]
    public class Exam : ISoftDeletableEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("total_mark")]
        public decimal TotalMark { get; set; } = 0;

        [Column("number_of_questions")]
        public int NumberOfQuestions { get; set; } = 0;

        [Column("duration")]
        public int? DurationInMinutes { get; set; }

        [Column("is_randomized")]
        public bool IsRandomized { get; set; } = false;

        [Column("pass_mark_percentage")]
        public int PassMarkPercentage { get; set; }

        [Column("exam_type")]
        public ExamType ExamType { get; set; } = ExamType.FixedTimeExam;

        [Column("status")]
        public ExamStatus Status { get; set; } = ExamStatus.Draft;

        [Column("start_time")]
        public DateTimeOffset? StartTime { get; set; }

        [Column("end_time")]
        public DateTimeOffset? EndTime { get; set; }

        [Column("course_id")]
        [ForeignKey(nameof(Course))]
        public Guid CourseId { get; set; }

        [Column("section_id")]
        [ForeignKey(nameof(Section))]
        public Guid? SectionId { get; set; }

        [Column("instructor_id")]
        [ForeignKey(nameof(Instructor))]
        public Guid InstructorId { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        public Course? Course { get; set; }
        public Section? Section { get; set; }
        public Instructor Instructor { get; set; } = null!;
        public ICollection<ExamBank> ExamQuestions { get; set; } = [];
        public ICollection<StudentExam> StudentExams { get; set; } = [];
        public ICollection<StudentExamResult> ExamResults { get; set; } = [];
    }
}


