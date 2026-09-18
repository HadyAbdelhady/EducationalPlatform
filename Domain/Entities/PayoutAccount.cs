using Domain.enums;
using Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("payout_accounts", Schema = "public")]
    public class PayoutAccount : IEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        /// <summary>Center or Instructor — discriminator for polymorphic payee.</summary>
        [Column("payee_type")]
        public PayeeType PayeeType { get; set; }

        /// <summary>CenterId or Instructor.UserId depending on PayeeType.</summary>
        [Column("payee_id")]
        public Guid PayeeId { get; set; }

        /// <summary>Running balance in EGP. May be negative (unpaid subscription debt).</summary>
        [Column("balance")]
        public decimal Balance { get; set; }

        /// <summary>Paymob Send recipient id — null until KYC is complete.</summary>
        [Column("paymob_recipient_id")]
        public string? PaymobRecipientId { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; } = EgyptTime.UtcNow;

        [Column("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
