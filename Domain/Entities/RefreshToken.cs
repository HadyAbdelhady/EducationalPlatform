using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    [Table("refresh_tokens", Schema = "public")]
    public class RefreshToken : IEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } 

        [Required]
        [Column("token")]
        public string Token { get; set; } = string.Empty;

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }

        public User User { get; set; } = null!;
    }
}
