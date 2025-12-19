using Domain.enums;
using Domain.Interfaces;

namespace Domain.Entities
{

    public class StudentExamResult : ISoftDeletableEntity
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid ExamId { get; set; }
        public decimal? StudentMark { get; set; }
        public ExamStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public Student Student { get; set; } = null!;
        public Exam Exam { get; set; } = null!;

        public ICollection<StudentSubmission> StudentSubmissions { get; set; } = null!;
    }
}