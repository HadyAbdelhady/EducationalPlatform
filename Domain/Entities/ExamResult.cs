using System;
using System.Collections.Generic;

namespace Domain.Entities
{

 public class ExamResult
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid ExamId { get; set; }
        public decimal? TotalMark { get; set; }
        public DateTimeOffset SubmittingDate { get; set; }
        public ExamStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public Student Student { get; set; } = null!;
        public Exam Exam { get; set; } = null!;
    }
}