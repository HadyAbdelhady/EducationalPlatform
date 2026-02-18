using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    [Table("answers", Schema = "public")]
    public class Answer : ISoftDeletableEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("question_id")]
        [ForeignKey(nameof(Question))]
        public Guid? QuestionId { get; set; }

        [Required]
        [Column("answer_text")]
        public string AnswerText { get; set; } = string.Empty;

        [Column("is_correct")]
        public bool IsCorrect { get; set; }

        [Column("explanation")]
        public string? Explanation { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        [Column("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        public Question? Question { get; set; }
    }
}


