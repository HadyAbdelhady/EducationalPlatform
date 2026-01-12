using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.enums;
using Domain.Interfaces;

namespace Domain.Entities
{

    [Table("student_exam_results", Schema = "public")]
    public class StudentExamResult : ISoftDeletableEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("student_id")]
        [ForeignKey(nameof(Student))]
        public Guid StudentId { get; set; }

        [Column("exam_id")]
        [ForeignKey(nameof(Exam))]
        public Guid ExamId { get; set; }

        [Column("obtained_marks")]
        public decimal? StudentMark { get; set; }
        [Column("exam_result_status")]
        public ExamResultStatus Status { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        public Student Student { get; set; } = null!;
        public Exam Exam { get; set; } = null!;

        public ICollection<StudentAnswers> StudentSubmissions { get; set; } = null!;
    }
}