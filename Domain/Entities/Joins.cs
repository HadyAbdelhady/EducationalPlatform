using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    [Table("student_courses", Schema = "public")]
    public class StudentCourse 
    {
        [Key]
        [Column("student_id")]
        [ForeignKey(nameof(Student))]
        public Guid StudentId { get; set; }

        [Key]
        [Column("course_id")]
        [ForeignKey(nameof(Course))]
        public Guid CourseId { get; set; }

        [Column("enrolled_at")]
        public DateTimeOffset EnrolledAt { get; set; }

        [Column("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        [Column("number_of_course_videos_watched")]
        public int NumberOfCourseVideosWatched { get; set; }

        public Student Student { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }

    [Table("student_sections", Schema = "public")]
    public class StudentSection
    {
        [Key]
        [Column("student_id")]
        [ForeignKey(nameof(Student))]
        public Guid StudentId { get; set; }

        [Key]
        [Column("section_id")]
        [ForeignKey(nameof(Section))]
        public Guid SectionId { get; set; }

        [Column("enrolled_at")]
        public DateTimeOffset EnrolledAt { get; set; }

        [Column("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [Column("number_of_section_videos_watched")]
        public int NumberOfSectionVideosWatched { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        public Student Student { get; set; } = null!;
        public Section Section { get; set; } = null!;
    }

    [Table("student_videos", Schema = "public")]
    public class StudentVideo
    {
        [Key]
        [Column("student_id")]
        [ForeignKey(nameof(Student))]
        public Guid StudentId { get; set; }

        [Key]
        [Column("video_id")]
        [ForeignKey(nameof(Video))]
        public Guid VideoId { get; set; }

        [Column("watched_at")]
        public DateTimeOffset WatchedAt { get; set; }

        [Column("is_watched")]
        public bool IsWatched { get; set; }

        [Column("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        public Student Student { get; set; } = null!;
        public Video Video { get; set; } = null!;
    }

    //public class StudentSheet
    //{
    //    public Guid StudentId { get; set; }
    //    public Guid SheetId { get; set; }
    //    public DateTimeOffset ViewedAt { get; set; }
    //    public DateTimeOffset UpdatedAt { get; set; }
    //    public bool IsDeleted { get; set; }

    //    public Student Student { get; set; } = null!;
    //    public SheetBase SheetBase { get; set; } = null!;
    //}

    [Table("student_exams", Schema = "public")]
    public class StudentExam
    {
        [Key]
        [Column("student_id")]
        [ForeignKey(nameof(Student))]
        public Guid StudentId { get; set; }

        [Key]
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
        [Key]
        [Column("instructor_id")]
        [ForeignKey(nameof(Instructor))]
        public Guid InstructorId { get; set; }

        [Key]
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
        [Key]
        [Column("instructor_id")]
        [ForeignKey(nameof(Instructor))]
        public Guid InstructorId { get; set; }

        [Key]
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

    [Table("instructor_exams", Schema = "public")]
    public class InstructorExam
    {
        [Key]
        [Column("instructor_id")]
        [ForeignKey(nameof(Instructor))]
        public Guid InstructorId { get; set; }

        [Key]
        [Column("exam_id")]
        [ForeignKey(nameof(Exam))]
        public Guid ExamId { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        [Column("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        public Instructor Instructor { get; set; } = null!;
        public Exam Exam { get; set; } = null!;
    }

    //public class VideoSheet
    //{
    //    public Guid CourseId { get; set; }
    //    public Guid SheetId { get; set; }
    //    public DateTimeOffset UpdatedAt { get; set; }
    //    public bool IsDeleted { get; set; }

    //    public Video Video { get; set; } = null!;
    //    public SheetBase SheetBase { get; set; } = null!;
    //}
}


