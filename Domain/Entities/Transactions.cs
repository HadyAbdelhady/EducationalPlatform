using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    [Table("payments", Schema = "public")]
    public class Payment : ISoftDeletableEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("student_id")]
        [ForeignKey(nameof(Student))]
        public Guid? StudentId { get; set; }

        [Column("status")]
        public PaymentStatus Status { get; set; }

        [Column("course_id")]
        [ForeignKey(nameof(Course))]
        public Guid? CourseId { get; set; }

        [Column("section_id")]
        [ForeignKey(nameof(Section))]
        public Guid? SectionId { get; set; }

        [Required]
        [Column("sender_account")]
        public string SenderAccount { get; set; } = string.Empty;

        [Required]
        [Column("receiver_account")]
        public string ReceiverAccount { get; set; } = string.Empty;

        [Column("commission_account1")]
        public string? CommissionAccount1 { get; set; }

        [Column("commission_account2")]
        public string? CommissionAccount2 { get; set; }

        [Column("time_of_payment")]
        public DateTimeOffset TimeOfPayment { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

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


