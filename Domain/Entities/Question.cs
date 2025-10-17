using System;
using System.Collections.Generic;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class Question : ISoftDeletableEntity
    {
        public Guid Id { get; set; }
        public string QuestionString { get; set; } = string.Empty;
        public string? QuestionImageUrl { get; set; }
        public decimal? QuestionMark { get; set; }
        public Guid? ExamId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public Exam? Exam { get; set; }
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
        public ICollection<StudentSubmission> StudentSubmissions { get; set; } = new List<StudentSubmission>();
    }
}


