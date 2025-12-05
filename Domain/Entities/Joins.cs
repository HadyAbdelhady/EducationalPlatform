namespace Domain.Entities
{
    public class StudentCourse 
    {
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public DateTimeOffset EnrolledAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public int NumberOfCourseVideosWatched { get; set; }

        public Student Student { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }

    public class StudentSection
    {
        public Guid StudentId { get; set; }
        public Guid SectionId { get; set; }
        public DateTimeOffset EnrolledAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public int NumberOfSectionVideosWatched { get; set; }

        public bool IsDeleted { get; set; }

        public Student Student { get; set; } = null!;
        public Section Section { get; set; } = null!;
    }

    public class StudentVideo
    {
        public Guid StudentId { get; set; }
        public Guid VideoId { get; set; }
        public DateTimeOffset WatchedAt { get; set; }
        public bool IsWatched { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public Student Student { get; set; } = null!;
        public Video Video { get; set; } = null!;
    }

    public class StudentSheet
    {
        public Guid StudentId { get; set; }
        public Guid SheetId { get; set; }
        public DateTimeOffset ViewedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public Student Student { get; set; } = null!;
        public Sheet Sheet { get; set; } = null!;
    }

    public class StudentExam
    {
        public Guid StudentId { get; set; }
        public Guid ExamId { get; set; }
        public DateTimeOffset TakenAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public Student Student { get; set; } = null!;
        public Exam Exam { get; set; } = null!;
    }

    public class InstructorCourse 
    {
        public Guid InstructorId { get; set; }
        public Guid CourseId { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public Instructor Instructor { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }

    public class InstructorSection
    {
        public Guid InstructorId { get; set; }
        public Guid SectionId { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        public Instructor Instructor { get; set; } = null!;
        public Section Section { get; set; } = null!;
    }

    public class InstructorExam
    {
        public Guid InstructorId { get; set; }
        public Guid ExamId { get; set; }
        public bool IsDeleted { get; set; }

        public Instructor Instructor { get; set; } = null!;
        public Exam Exam { get; set; } = null!;
    }

    public class VideoSheet
    {
        public Guid VideoId { get; set; }
        public Guid SheetId { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public Video Video { get; set; } = null!;
        public Sheet Sheet { get; set; } = null!;
    }
}


