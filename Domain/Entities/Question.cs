using Domain.Interfaces;

namespace Domain.Entities
{
    public class Question : ISoftDeletableEntity
    {
        public Guid Id { get; set; }
        public Guid SectionId { get; set; }
        public Guid CourseId { get; set; }
        public string QuestionString { get; set; } = string.Empty;
        public string? QuestionImageUrl { get; set; }
        public decimal? QuestionMark { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
        public bool IsDeleted { get; set; } = false;

        public Section? Section { get; set; } = null;
        public Course? Course { get; set; } = null;
        public ICollection<Answer> Answers { get; set; } = [];
        public ICollection<StudentSubmission> StudentSubmissions { get; set; } = [];
    }
}


