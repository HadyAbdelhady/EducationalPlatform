using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{

    [Table("student_answers", Schema = "public")]
    public class StudentAnswers
    {
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

        public Question Question { get; set; } = null!;
        public Answer? ChosenAnswer { get; set; }
        public StudentExamResult ExamResult { get; set; } = null!;

        public Student Student { get; set; } = null!;
    }
}