using Domain.enums;
using Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("payout_batches", Schema = "public")]
    public class PayoutBatch : IEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("payee_type")]
        public PayeeType PayeeType { get; set; }

        [Column("payee_id")]
        public Guid PayeeId { get; set; }

        [Column("period_year")]
        public int PeriodYear { get; set; }

        [Column("period_month")]
        public int PeriodMonth { get; set; }

        /// <summary>Sum of SaleCredit ledger lines included in this batch.</summary>
        [Column("credits")]
        public decimal Credits { get; set; }

        /// <summary>100 EGP for active payees; 0 if waived (future use).</summary>
        [Column("subscription_fee")]
        public decimal SubscriptionFee { get; set; }

        /// <summary>openingBalance + credits + refundReversals - subscriptionFee.</summary>
        [Column("net")]
        public decimal Net { get; set; }

        [Column("paymob_send_id")]
        public string? PaymobSendId { get; set; }

        [Column("status")]
        public PayoutBatchStatus Status { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; } = EgyptTime.UtcNow;

        [Column("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        public ICollection<PaymentTransactions> Payments { get; set; } = [];
        public ICollection<PayoutLedgerLine> LedgerLines { get; set; } = [];
    }
}
