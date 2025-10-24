using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Instructor 
    {
        public Guid UserId { get; set; }

        public User User { get; set; } = null!;

        public ICollection<InstructorCourse> InstructorCourses { get; set; } = [];
        public ICollection<InstructorSection> InstructorSections { get; set; } = [];
        public ICollection<InstructorExam> InstructorExams { get; set; } = [];
    }
}


