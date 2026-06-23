using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("center_admins", Schema = "public")]
    public class CenterAdmin
    {
        [Key]
        [Column("user_id")]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [Column("center_id")]
        [ForeignKey(nameof(Center))]
        public Guid CenterId { get; set; }

        public User User { get; set; } = null!;
        public Center Center { get; set; } = null!;
    }
}
