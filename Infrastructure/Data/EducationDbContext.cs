using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class EducationDbContext(DbContextOptions<EducationDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Instructor> Instructors => Set<Instructor>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Section> Sections => Set<Section>();
        public DbSet<Video> Videos => Set<Video>();
        public DbSet<Sheet> Sheets => Set<Sheet>();
        public DbSet<Exam> Exams => Set<Exam>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<Answer> Answers => Set<Answer>();
        public DbSet<StudentCourse> StudentCourses => Set<StudentCourse>();
        public DbSet<StudentSection> StudentSections => Set<StudentSection>();
        public DbSet<StudentVideo> StudentVideos => Set<StudentVideo>();
        public DbSet<StudentSheet> StudentSheets => Set<StudentSheet>();
        public DbSet<StudentExam> StudentExams => Set<StudentExam>();
        public DbSet<InstructorCourse> InstructorCourses => Set<InstructorCourse>();
        public DbSet<InstructorSection> InstructorSections => Set<InstructorSection>();
        public DbSet<InstructorExam> InstructorExams => Set<InstructorExam>();
        public DbSet<VideoSheet> VideoSheets => Set<VideoSheet>();
        public DbSet<CourseReview> CourseReviews => Set<CourseReview>();
        public DbSet<SectionReview> SectionReviews => Set<SectionReview>();
        public DbSet<VideoReview> VideoReviews => Set<VideoReview>();
        public DbSet<ChatRoom> ChatRooms => Set<ChatRoom>();
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<ExamResult> ExamResults => Set<ExamResult>();
        public DbSet<StudentSubmission> StudentSubmissions => Set<StudentSubmission>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            EducationDbContextModelCreating.Apply(modelBuilder);
        }
    }
}
