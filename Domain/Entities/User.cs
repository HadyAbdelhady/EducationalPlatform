using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    [Table("users", Schema = "public")]
    public class User : ISoftDeletableEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("full_name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Column("ssn")]
        public string Ssn { get; set; } = string.Empty;

        [Required]
        [Column("phone_number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Column("gmail_external")]
        public string? GmailExternal { get; set; }

        [Column("personal_picture_url")]
        public string? PersonalPictureUrl { get; set; }

        [Column("date_of_birth")]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        [Column("gender")]
        public string Gender { get; set; } = string.Empty;

        [Column("location_maps")]
        public string? LocationMaps { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        public Student? Student { get; set; }
        public Instructor? Instructor { get; set; }
    }
}


