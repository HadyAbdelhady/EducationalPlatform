using System.Text.Json.Serialization;

namespace Application.DTOs.Payment.PaymobRawDtos
{
    public class PaymobWebhookPayload
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("obj")]
        public PaymobWebhookObj Obj { get; set; } = null!;

        [JsonPropertyName("issuer_bank")]
        public string? IssuerBank { get; set; }

        [JsonPropertyName("transaction_processed_callback_responses")]
        public string? TransactionProcessedCallbackResponses { get; set; }
    }

    public class PaymobWebhookObj
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("pending")]
        public bool Pending { get; set; }

        [JsonPropertyName("amount_cents")]
        public int AmountCents { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("is_auth")]
        public bool IsAuth { get; set; }

        [JsonPropertyName("is_capture")]
        public bool IsCapture { get; set; }

        [JsonPropertyName("is_standalone_payment")]
        public bool IsStandalonePayment { get; set; }

        [JsonPropertyName("is_voided")]
        public bool IsVoided { get; set; }

        [JsonPropertyName("is_refunded")]
        public bool IsRefunded { get; set; }

        [JsonPropertyName("is_3d_secure")]
        public bool Is3DSecure { get; set; }

        [JsonPropertyName("integration_id")]
        public int IntegrationId { get; set; }

        [JsonPropertyName("profile_id")]
        public int ProfileId { get; set; }

        [JsonPropertyName("has_parent_transaction")]
        public bool HasParentTransaction { get; set; }

        [JsonPropertyName("order")]
        public PaymobWebhookOrder? Order { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; } = string.Empty;

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonPropertyName("source_data")]
        public PaymobWebhookSourceData? SourceData { get; set; }

        [JsonPropertyName("api_source")]
        public string ApiSource { get; set; } = string.Empty;

        [JsonPropertyName("terminal_id")]
        public string? TerminalId { get; set; }

        [JsonPropertyName("merchant_commission")]
        public int? MerchantCommission { get; set; }

        [JsonPropertyName("installment")]
        public string? Installment { get; set; }

        [JsonPropertyName("is_void")]
        public bool IsVoid { get; set; }

        [JsonPropertyName("is_refund")]
        public bool IsRefund { get; set; }

        [JsonPropertyName("data")]
        public PaymobWebhookData? Data { get; set; }

        [JsonPropertyName("is_hidden")]
        public bool IsHidden { get; set; }

        [JsonPropertyName("payment_key_claims")]
        public PaymobWebhookPaymentKeyClaims? PaymentKeyClaims { get; set; }

        [JsonPropertyName("error_occured")]
        public bool ErrorOccured { get; set; }

        [JsonPropertyName("is_live")]
        public bool IsLive { get; set; }

        [JsonPropertyName("other_endpoint_reference")]
        public string? OtherEndpointReference { get; set; }

        [JsonPropertyName("refunded_amount_cents")]
        public int? RefundedAmountCents { get; set; }

        [JsonPropertyName("source_id")]
        public int SourceId { get; set; }

        [JsonPropertyName("is_captured")]
        public bool IsCaptured { get; set; }

        [JsonPropertyName("captured_amount")]
        public int? CapturedAmount { get; set; }

        [JsonPropertyName("merchant_staff_tag")]
        public string? MerchantStaffTag { get; set; }

        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set; } = string.Empty;

        [JsonPropertyName("is_settled")]
        public bool IsSettled { get; set; }

        [JsonPropertyName("bill_balanced")]
        public bool BillBalanced { get; set; }

        [JsonPropertyName("is_bill")]
        public bool IsBill { get; set; }

        [JsonPropertyName("owner")]
        public int Owner { get; set; }

        [JsonPropertyName("parent_transaction")]
        public string? ParentTransaction { get; set; }
    }

    public class PaymobWebhookOrder
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; } = string.Empty;

        [JsonPropertyName("delivery_needed")]
        public bool DeliveryNeeded { get; set; }

        [JsonPropertyName("merchant")]
        public PaymobWebhookMerchant? Merchant { get; set; }

        [JsonPropertyName("collector")]
        public string? Collector { get; set; }

        [JsonPropertyName("amount_cents")]
        public int AmountCents { get; set; }

        [JsonPropertyName("shipping_data")]
        public BillingData? ShippingData { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonPropertyName("is_payment_locked")]
        public bool IsPaymentLocked { get; set; }

        [JsonPropertyName("is_return")]
        public bool IsReturn { get; set; }

        [JsonPropertyName("is_cancel")]
        public bool IsCancel { get; set; }

        [JsonPropertyName("is_returned")]
        public bool IsReturned { get; set; }

        [JsonPropertyName("is_canceled")]
        public bool IsCanceled { get; set; }

        [JsonPropertyName("merchant_order_id")]
        public string? MerchantOrderId { get; set; }

        [JsonPropertyName("wallet_notification")]
        public string? WalletNotification { get; set; }

        [JsonPropertyName("paid_amount_cents")]
        public int PaidAmountCents { get; set; }

        [JsonPropertyName("notify_user_with_email")]
        public bool NotifyUserWithEmail { get; set; }

        [JsonPropertyName("order_url")]
        public string OrderUrl { get; set; } = string.Empty;

        [JsonPropertyName("commission_fees")]
        public int? CommissionFees { get; set; }

        [JsonPropertyName("delivery_fees_cents")]
        public int? DeliveryFeesCents { get; set; }

        [JsonPropertyName("delivery_vat_cents")]
        public int? DeliveryVatCents { get; set; }

        [JsonPropertyName("payment_method")]
        public string PaymentMethod { get; set; } = string.Empty;

        [JsonPropertyName("merchant_staff_tag")]
        public string? MerchantStaffTag { get; set; }

        [JsonPropertyName("api_source")]
        public string ApiSource { get; set; } = string.Empty;
    }

    public class PaymobWebhookMerchant
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; } = string.Empty;

        [JsonPropertyName("company_emails")]
        public string[]? CompanyEmails { get; set; }

        [JsonPropertyName("company_name")]
        public string CompanyName { get; set; } = string.Empty;

        [JsonPropertyName("state")]
        public string State { get; set; } = string.Empty;

        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;

        [JsonPropertyName("postal_code")]
        public string PostalCode { get; set; } = string.Empty;

        [JsonPropertyName("street")]
        public string Street { get; set; } = string.Empty;
    }


    public class PaymobWebhookSourceData
    {
        [JsonPropertyName("pan")]
        public string Pan { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("tenure")]
        public string? Tenure { get; set; }

        [JsonPropertyName("sub_type")]
        public string SubType { get; set; } = string.Empty;
    }

    public class PaymobWebhookData
    {
        [JsonPropertyName("gateway_integration_pk")]
        public int GatewayIntegrationPk { get; set; }

        [JsonPropertyName("klass")]
        public string Klass { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonPropertyName("migs_result")]
        public string MigsResult { get; set; } = string.Empty;

        [JsonPropertyName("txn_response_code")]
        public string TxnResponseCode { get; set; } = string.Empty;

        [JsonPropertyName("acq_response_code")]
        public string AcqResponseCode { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("merchant_txn_ref")]
        public string MerchantTxnRef { get; set; } = string.Empty;

        [JsonPropertyName("order_info")]
        public string OrderInfo { get; set; } = string.Empty;

        [JsonPropertyName("receipt_no")]
        public string ReceiptNo { get; set; } = string.Empty;

        [JsonPropertyName("transaction_no")]
        public string TransactionNo { get; set; } = string.Empty;

        [JsonPropertyName("batch_no")]
        public int BatchNo { get; set; }

        [JsonPropertyName("authorize_id")]
        public string AuthorizeId { get; set; } = string.Empty;

        [JsonPropertyName("card_type")]
        public string CardType { get; set; } = string.Empty;

        [JsonPropertyName("card_num")]
        public string CardNum { get; set; } = string.Empty;

        [JsonPropertyName("secure_hash")]
        public string SecureHash { get; set; } = string.Empty;

        [JsonPropertyName("avs_result_code")]
        public string AvsResultCode { get; set; } = string.Empty;

        [JsonPropertyName("avs_acq_response_code")]
        public string AvsAcqResponseCode { get; set; } = string.Empty;

        [JsonPropertyName("captured_amount")]
        public int CapturedAmount { get; set; }

        [JsonPropertyName("authorised_amount")]
        public int AuthorisedAmount { get; set; }

        [JsonPropertyName("refunded_amount")]
        public int? RefundedAmount { get; set; }

        [JsonPropertyName("acs_eci")]
        public string AcsEci { get; set; } = string.Empty;
    }

    public class PaymobWebhookPaymentKeyClaims
    {
        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonPropertyName("order_id")]
        public int OrderId { get; set; }

        [JsonPropertyName("amount_cents")]
        public int AmountCents { get; set; }

        [JsonPropertyName("billing_data")]
        public BillingData? BillingData { get; set; } // Reused shipping data as it has same structure

        [JsonPropertyName("integration_id")]
        public int IntegrationId { get; set; }

        [JsonPropertyName("lock_order_when_paid")]
        public bool LockOrderWhenPaid { get; set; }

        [JsonPropertyName("next_payment_intention")]
        public string NextPaymentIntention { get; set; } = string.Empty;

        [JsonPropertyName("single_payment_attempt")]
        public bool SinglePaymentAttempt { get; set; }
    }
}
