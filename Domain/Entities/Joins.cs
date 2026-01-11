using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("student_courses", Schema = "public")]
    public class StudentCourse
    {
        [Column("student_id")]
        [ForeignKey(nameof(Student))]
        public Guid StudentId { get; set; }

        [Column("course_id")]
        [ForeignKey(nameof(Course))]
        public Guid CourseId { get; set; }


        [Column("number_of_course_videos_watched")]
        public int NumberOfCourseVideosWatched { get; set; } = 0;

        [Column("progress")]
        public decimal? Progress { get; set; }

        [Column("enrolled_at")]
        public DateTimeOffset EnrolledAt { get; set; } = DateTimeOffset.UtcNow;

        [Column("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        public Student Student { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }

    [Table("student_sections", Schema = "public")]
    public class StudentSection
    {
        [Column("student_id")]
        [ForeignKey(nameof(Student))]
        public Guid StudentId { get; set; }

        [Column("section_id")]
        [ForeignKey(nameof(Section))]
        public Guid SectionId { get; set; }

        [Column("enrolled_at")]
        public DateTimeOffset EnrolledAt { get; set; } = DateTimeOffset.UtcNow;

        [Column("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [Column("number_of_section_videos_watched")]
        public int NumberOfSectionVideosWatched { get; set; } = 0;

        [Column("progress")]
        public decimal? Progress { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        public Student Student { get; set; } = null!;
        public Section Section { get; set; } = null!;
    }

    [Table("student_videos", Schema = "public")]
    public class StudentVideo
    {
        [Column("student_id")]
        [ForeignKey(nameof(Student))]
        public Guid StudentId { get; set; }

        [Column("video_id")]
        [ForeignKey(nameof(Video))]
        public Guid VideoId { get; set; }

        [Column("watched_at")]
        public DateTimeOffset WatchedAt { get; set; }

        [Column("progress")]
        [Range(0, 100)]
        public int Progress { get; set; }

        [Column("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        public Student Student { get; set; } = null!;
        public Video Video { get; set; } = null!;
    }

    [Table("student_exams", Schema = "public")]
    public class StudentExam
    {
        [Column("student_id")]
        [ForeignKey(nameof(Student))]
        public Guid StudentId { get; set; }

        [Column("exam_id")]
        [ForeignKey(nameof(Exam))]
        public Guid ExamId { get; set; }

        [Column("taken_at")]
        public DateTimeOffset TakenAt { get; set; }

        [Column("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        public Student Student { get; set; } = null!;
        public Exam Exam { get; set; } = null!;
    }

    [Table("instructor_courses", Schema = "public")]
    public class InstructorCourse
    {
        [Column("instructor_id")]
        [ForeignKey(nameof(Instructor))]
        public Guid InstructorId { get; set; }

        [Column("course_id")]
        [ForeignKey(nameof(Course))]
        public Guid CourseId { get; set; }

        [Column("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        public Instructor Instructor { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }

    [Table("instructor_sections", Schema = "public")]
    public class InstructorSection
    {
        [Column("instructor_id")]
        [ForeignKey(nameof(Instructor))]
        public Guid InstructorId { get; set; }

        [Column("section_id")]
        [ForeignKey(nameof(Section))]
        public Guid SectionId { get; set; }

        [Column("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        public Instructor Instructor { get; set; } = null!;
        public Section Section { get; set; } = null!;
    }
}


