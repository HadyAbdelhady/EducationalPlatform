namespace Application.DTOs.Payment.PaymobRawDtos
{
    public class HMACStringKeys
    {
        public int AmountCents { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string Currency { get; set; } = string.Empty;
        public bool ErrorOccured { get; set; }
        public bool HasParentTransaction { get; set; }
        public string ObjId { get; set; } = string.Empty;  // for Processed (POST) | id for Response (GET)
        public string IntegrationId { get; set; } = string.Empty;
        public bool Is3DSecure { get; set; }
        public bool IsAuth { get; set; }
        public bool IsCapture { get; set; }
        public bool IsRefunded { get; set; }
        public bool IsStandalonePayment { get; set; }
        public bool IsVoided { get; set; }
        public string OrderId { get; set; } = string.Empty;  // for Processed (POST) | order_id for Response (GET)
        public string Owner { get; set; } = string.Empty;
        public bool Pending { get; set; }
        public string SourceDataPan { get; set; } = string.Empty;
        public string SourceDataSubType { get; set; } = string.Empty;
        public string SourceDataType { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}