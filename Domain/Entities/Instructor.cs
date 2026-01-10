using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("instructors", Schema = "public")]
    public class Instructor
    {
        [Key]
        [Column("user_id")]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public User User { get; set; } = null!;

        public ICollection<InstructorCourse> InstructorCourses { get; set; } = [];
        public ICollection<InstructorSection> InstructorSections { get; set; } = [];
        public ICollection<InstructorReview> InstructorReviews { get; set; } = [];
        public ICollection<Exam> Exams { get; set; } = [];
        public ICollection<Sheet> Sheets { get; set; } = [];
    }
}


