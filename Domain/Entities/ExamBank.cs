using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("exam_bank", Schema = "public")]
    public class ExamBank
    {
        [Key]
        [Column("exam_id")]
        [ForeignKey(nameof(Exam))]
        public Guid ExamId { get; set; }

        [Key]
        [Column("question_id")]
        [ForeignKey(nameof(Question))]
        public Guid QuestionId { get; set; }

        [Column("question_mark")]
        public decimal QuestionMark { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        public Exam Exam { get; set; } = null!;
        public Question Question { get; set; } = null!;
    }

}
