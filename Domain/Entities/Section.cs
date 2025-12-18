using Domain.Interfaces;

namespace Domain.Entities
{
    public class Section : ISoftDeletableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int NumberOfVideos { get; set; }
        public decimal? Rating { get; set; }
        public Guid? CourseId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public int NumberOfExams { get; set; }
        public Course? Course { get; set; }

        public ICollection<Video> Videos { get; set; } = [];
        public ICollection<Sheet> Sheets { get; set; } = [];
        public ICollection<InstructorSection> InstructorSections { get; set; } = [];
        public ICollection<StudentSection> StudentSections { get; set; } = [];
        public ICollection<SectionReview> SectionReviews { get; set; } = [];
        public ICollection<Exam> Exams { get; set; } = [];
        public ICollection<Payment> Payments { get; set; } = [];
    }
}


