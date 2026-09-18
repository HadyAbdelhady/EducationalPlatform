using Domain.enums;
using Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("payout_ledger_lines", Schema = "public")]
    public class PayoutLedgerLine : IEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("payee_type")]
        public PayeeType PayeeType { get; set; }

        [Column("payee_id")]
        public Guid PayeeId { get; set; }

        [Column("type")]
        public LedgerLineType Type { get; set; }

        /// <summary>
        /// Positive for SaleCredit; negative for RefundReversal and MonthlySubscription.
        /// </summary>
        [Column("amount")]
        public decimal Amount { get; set; }

        /// <summary>Set for SaleCredit and RefundReversal lines; null for MonthlySubscription.</summary>
        [Column("payment_id")]
        [ForeignKey(nameof(Payment))]
        public Guid? PaymentId { get; set; }

        /// <summary>Null until the monthly job attaches this line to a batch.</summary>
        [Column("payout_batch_id")]
        [ForeignKey(nameof(PayoutBatch))]
        public Guid? PayoutBatchId { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; } = EgyptTime.UtcNow;

        public PaymentTransactions? Payment { get; set; }
        public PayoutBatch? PayoutBatch { get; set; }
    }
}
