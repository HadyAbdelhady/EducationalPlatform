using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    [Table("videos", Schema = "public")]
    public class Video : ISoftDeletableEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("video_url")]
        public string VideoUrl { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("section_id")]
        [ForeignKey(nameof(Section))]
        public Guid? SectionId { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        public Section? Section { get; set; }

        public ICollection<Sheet> Sheets { get; set; } = [];
        public ICollection<StudentVideo> StudentVideos { get; set; } = new List<StudentVideo>();
        public ICollection<VideoReview> VideoReviews { get; set; } = new List<VideoReview>();
    }
}


