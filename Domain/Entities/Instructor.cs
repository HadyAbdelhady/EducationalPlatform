using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Instructor
    {
        public Guid UserId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public User User { get; set; } = null!;

        public ICollection<InstructorCourse> InstructorCourses { get; set; } = new List<InstructorCourse>();
        public ICollection<InstructorSection> InstructorSections { get; set; } = new List<InstructorSection>();
        public ICollection<InstructorExam> InstructorExams { get; set; } = new List<InstructorExam>();
    }
}


