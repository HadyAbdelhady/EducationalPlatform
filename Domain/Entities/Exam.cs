using Domain.enums;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class Exam : ISoftDeletableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal? TotalMark { get; set; } = 0;
        public int NumberOfQuestions { get; set; } = 0;
        public int? DurationInMinutes { get; set; }
        public bool IsRandomized { get; set; } = false;

        public ExamType ExamType { get; set; } = ExamType.FixedTimeExam;

        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }

        public Guid CourseId { get; set; }
        public Guid? SectionId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
        public Course? Course { get; set; }
        public Section? Section { get; set; }
        public ICollection<ExamBank> ExamQuestions { get; set; } = [];
        public ICollection<InstructorExam> InstructorExams { get; set; } = [];
        //public ICollection<Question> Questions { get; set; } = [];
        public ICollection<StudentExam> StudentExams { get; set; } = [];
        public ICollection<ExamResult> ExamResults { get; set; } = [];
    }
}


