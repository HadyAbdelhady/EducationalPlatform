using System;
using System.Collections.Generic;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class Exam : ISoftDeletableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public decimal? TotalMark { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? SectionId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public Course? Course { get; set; }
        public Section? Section { get; set; }

        public ICollection<InstructorExam> InstructorExams { get; set; } = new List<InstructorExam>();
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<StudentExam> StudentExams { get; set; } = new List<StudentExam>();
        public ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
    }
}


