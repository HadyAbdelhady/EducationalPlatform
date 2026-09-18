namespace Domain
{
    /// <summary>
    /// Pure money calculator — no EF, no DI, no side effects.
    /// All amounts in EGP (decimal). Paymob fees arrive in piastres (int) and are converted here.
    /// </summary>
    public static class PayoutMath
    {
        private const decimal PlatformRate = 0.10m;
        private const decimal RefundRate   = 0.90m;
        private const decimal PiastresPerEgp = 100m;

        /// <summary>
        /// Splits a completed payment into platform fee and payee credit.
        /// paymobFeePiastres: merchant_commission or commission_fees from the webhook (cents / piastres).
        /// Remainder goes to payee so that platformFee + paymobFee + payeeCredit == gross exactly.
        /// </summary>
        public static (decimal PlatformFee, decimal PaymobFee, decimal PayeeCredit) ComputeSaleCredit(
            decimal grossEgp, int paymobFeePiastres)
        {
            var platformFee = Math.Round(grossEgp * PlatformRate, 2, MidpointRounding.AwayFromZero);
            var paymobFee   = paymobFeePiastres / PiastresPerEgp;
            var payeeCredit = grossEgp - platformFee - paymobFee;
            return (platformFee, paymobFee, payeeCredit);
        }

        /// <summary>
        /// Net amount for a monthly batch.
        /// net = openingBalance + creditsInPeriod + refundReversalsInPeriod(negative) - subscriptionFee
        /// </summary>
        public static decimal ComputeMonthlyNet(
            decimal openingBalance,
            decimal creditsInPeriod,
            decimal refundReversalsInPeriod,
            decimal subscriptionFee)
            => openingBalance + creditsInPeriod + refundReversalsInPeriod - subscriptionFee;

        /// <summary>
        /// Amount to reverse from payee when a student refunds within 2 days.
        /// Does NOT reverse the platform cut or the Paymob acceptance fee.
        /// </summary>
        public static decimal ComputeRefundReversal(decimal grossEgp)
            => Math.Round(grossEgp * RefundRate, 2, MidpointRounding.AwayFromZero);

        /// <summary>
        /// Amount in piastres to send to Paymob refund API for a 90% refund.
        /// </summary>
        public static int ComputeRefundPiastres(decimal grossEgp)
            => (int)(ComputeRefundReversal(grossEgp) * PiastresPerEgp);
    }
}
