using System;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class Payment : ISoftDeletableEntity
    {
        public Guid Id { get; set; }
        public Guid? StudentId { get; set; }
        public PaymentStatus Status { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? SectionId { get; set; }
        public string SenderAccount { get; set; } = string.Empty;
        public string ReceiverAccount { get; set; } = string.Empty;
        public string? CommissionAccount1 { get; set; }
        public string? CommissionAccount2 { get; set; }
        public DateTimeOffset TimeOfPayment { get; set; }
        public decimal Amount { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public Student? Student { get; set; }
        public Course? Course { get; set; }
        public Section? Section { get; set; }
    }

}


