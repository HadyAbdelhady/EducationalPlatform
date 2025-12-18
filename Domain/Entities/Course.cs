using Domain.Interfaces;

namespace Domain.Entities
{
    public class Course : ISoftDeletableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? PictureUrl { get; set; }
        public string? IntroVideoUrl { get; set; }
        public int NumberOfVideos { get; set; } = 0;
        public int NumberOfSections { get; set; } = 0;
        public int NumberOfExams { get; set; } = 0;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<Section> Sections { get; set; } = [];
        public ICollection<InstructorCourse> InstructorCourses { get; set; } = [];
        public ICollection<StudentCourse> StudentCourses { get; set; } = [];
        public ICollection<CourseReview> CourseReviews { get; set; } = [];
        public ICollection<Exam> Exams { get; set; } = [];
        public ICollection<Payment> Payments { get; set; } = [];
    }
}


