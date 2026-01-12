using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{

    [Table("student_answers", Schema = "public")]
    public class StudentAnswers : ISoftDeletableEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("student_id")]
        [ForeignKey(nameof(Student))]
        public Guid StudentId { get; set; }

        [Column("exam_result_id")]
        [ForeignKey(nameof(ExamResult))]
        public Guid ExamResultId { get; set; }

        [Column("question_id")]
        [ForeignKey(nameof(Question))]
        public Guid QuestionId { get; set; }

        [Column("chosen_answer_id")]
        [ForeignKey(nameof(ChosenAnswer))]
        public Guid? ChosenAnswerId { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        public Question Question { get; set; } = null!;
        public Answer? ChosenAnswer { get; set; }
        public StudentExamResult ExamResult { get; set; } = null!;

        public Student Student { get; set; } = null!;
    }
}