using System;
using System.Collections.Generic;
using Domain.Interfaces;

namespace Domain.Entities
{
    
    public class StudentSubmission : ISoftDeletableEntity
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid? ChosenAnswerId { get; set; }
        public decimal? Mark { get; set; }
        public DateTimeOffset SubmittingDate { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public Student Student { get; set; } = null!;
        public Question Question { get; set; } = null!;
        public Answer? ChosenAnswer { get; set; }
    }
}