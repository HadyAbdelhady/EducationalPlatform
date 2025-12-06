using Domain.Interfaces;

namespace Domain.Entities
{
    public class Sheet : ISoftDeletableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? SheetUrl { get; set; }
        public Guid? SectionId { get; set; }
        public Guid? VideoId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public Section? Section { get; set; }
        public Video? Video { get; set; }

        public ICollection<StudentSheet> StudentSheets { get; set; } = [];
        public ICollection<VideoSheet> VideoSheets { get; set; } = [];
    }
}


