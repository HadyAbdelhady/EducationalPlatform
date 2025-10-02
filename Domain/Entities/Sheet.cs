using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Sheet
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? SheetUrl { get; set; }
        public DateTimeOffset DateOfCreation { get; set; }
        public Guid? SectionId { get; set; }
        public Guid? VideoId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public Section? Section { get; set; }
        public Video? Video { get; set; }

        public ICollection<StudentSheet> StudentSheets { get; set; } = new List<StudentSheet>();
        public ICollection<VideoSheet> VideoSheets { get; set; } = new List<VideoSheet>();
    }
}


