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
        public DateTimeOffset EnrolledAt { get; set; } = EgyptTime.UtcNow;

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
        public DateTimeOffset EnrolledAt { get; set; } = EgyptTime.UtcNow;

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

    // ── Center junction tables ────────────────────────────────────────────

    [Table("center_instructors", Schema = "public")]
    public class CenterInstructor
    {
        [Column("center_id")]
        [ForeignKey(nameof(Center))]
        public Guid CenterId { get; set; }

        [Column("instructor_id")]
        [ForeignKey(nameof(Instructor))]
        public Guid InstructorId { get; set; }

        [Column("joined_at")]
        public DateTimeOffset JoinedAt { get; set; } = EgyptTime.UtcNow;

        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        public Center Center { get; set; } = null!;
        public Instructor Instructor { get; set; } = null!;
    }

    [Table("center_instructor_education_years", Schema = "public")]
    public class CenterInstructorEducationYear
    {
        [Column("center_id")]
        [ForeignKey(nameof(Center))]
        public Guid CenterId { get; set; }

        [Column("instructor_id")]
        [ForeignKey(nameof(Instructor))]
        public Guid InstructorId { get; set; }

        [Column("education_year_id")]
        [ForeignKey(nameof(EducationYear))]
        public Guid EducationYearId { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("assigned_at")]
        public DateTimeOffset AssignedAt { get; set; } = EgyptTime.UtcNow;

        public Center Center { get; set; } = null!;
        public Instructor Instructor { get; set; } = null!;
        public EducationYear EducationYear { get; set; } = null!;
    }
}


