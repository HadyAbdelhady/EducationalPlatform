using Domain.enums;
using Domain.Interfaces;

namespace Domain.Entities
{
    public abstract class SheetBase : ISoftDeletableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SheetUrl { get; set; } = null!;
        public string SheetPublicId { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; } 
        public DateTimeOffset? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class AnswersSheet : SheetBase
    {
        public Guid QuestionsSheetId { get; set; }
        public Guid StudentId { get; set; }
        public bool IsApproved  { get; set; }
        public Sheet QuestionsSheet { get; set; } = null!;
        public Student Student { get; set; } = null!;
    }

    public class Sheet : SheetBase
    {
        public Guid? SectionId { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? VideoId { get; set; }
        public Guid InstructorId { get; set; }
        public SheetType Type { get; set; }
        public DateTime? DueDate { get; set; }
        public Section? Section { get; set; }
        public Video? Video { get; set; }
        public Course? Course { get; set; }
        public Instructor Instructor { get; set; } = null!;
        public ICollection<AnswersSheet> AnswersSheets { get; set; } = [];
    }
}


