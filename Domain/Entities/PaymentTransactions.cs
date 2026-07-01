using Domain.enums;
using Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("payments", Schema = "public")]
    public class PaymentTransactions : ISoftDeletableEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("student_id")]
        [ForeignKey(nameof(Student))]
        public Guid StudentId { get; set; }

        [Column("status")]
        public PaymentStatus Status { get; set; }

        [Column("course_id")]
        [ForeignKey(nameof(Course))]
        public Guid? CourseId { get; set; }

        [Column("section_id")]
        [ForeignKey(nameof(Section))]
        public Guid? SectionId { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("paymob_intention_id")]
        public string? PaymobIntentionId { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        public Student? Student { get; set; }
        public Course? Course { get; set; }
        public Section? Section { get; set; }
    }

}


