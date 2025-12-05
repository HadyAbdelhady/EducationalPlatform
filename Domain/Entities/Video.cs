using System;
using System.Collections.Generic;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class Video : ISoftDeletableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        //public DateTimeOffset DateOfCreation { get; set; }
        public string? Description { get; set; }
        public Guid? SectionId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public Section? Section { get; set; }

        public ICollection<VideoSheet> VideoSheets { get; set; } = new List<VideoSheet>();
        public ICollection<StudentVideo> StudentVideos { get; set; } = new List<StudentVideo>();
        public ICollection<VideoReview> VideoReviews { get; set; } = new List<VideoReview>();
    }
}


