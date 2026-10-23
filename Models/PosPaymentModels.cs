namespace Suvarna.DGMS.Api.Models
{
    // =====================================================
    // PayLink settings of the login location (System Parameters DGMS_PAYLINK_*, see PayLinkSettings)
    // =====================================================
    public class PayLinkOptions
    {
        public bool Enabled { get; set; } = true;

        public string AuthUrl { get; set; } = "";
        public string PushUrl { get; set; } = "";
        public string StatusUrl { get; set; } = "";
        public string CancelUrl { get; set; } = "";      // empty = no cancel call, the machine auto-cancels

        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";

        public int TimeoutSeconds { get; set; } = 30;

        // PayLink token is reused for this many minutes (0 = new token for every call)
        public int TokenCacheMinutes { get; set; } = 10;

        public string Currency { get; set; } = "INR";

        // PayLink "txntype" per DGMS payment mode. Empty = that mode is not taken on the POS machine.
        public Dictionary<string, string> TxnTypes { get; set; } = new(StringComparer.OrdinalIgnoreCase) { ["CARD"] = "00", ["UPI"] = "01" };

        // Used when the device has no AUTO_CANCEL_DURATION_MINUTES
        public int DefaultAutoCancelMinutes { get; set; } = 5;

        // transstatus "status false" whose message contains one of these = guest has not finished yet
        // (stays waiting). Any other "status false" = FAILED: cancelled on the machine, machine freed.
        public List<string> PendingMessages { get; set; } = new() { "pending", "in progress", "initiated", "awaiting", "processing", "not completed yet" };

        // true  = a card RRN can still be typed by hand (POS machine down)
        // false = card payments must go through the POS machine
        public bool AllowManualCardEntry { get; set; } = true;
    }

    // =====================================================
    // POS machine (INT_PR_GET_PINE_LAB_MISSION_DATA row). Never sent to the browser.
    // =====================================================
    public class PineLabMissionData
    {
        public int CONFIG_ID { get; set; }
        public string? MAP_TERMINAL_NAME { get; set; }
        public string? DEVICE_NO { get; set; }
        public string? SECURITY_TOKEN { get; set; }
        public string? MERCHANT_STORE_POS_CODE { get; set; }
        public string? IMEI { get; set; }
        public int? AUTO_CANCEL_DURATION_MINUTES { get; set; }
        public string? MERCHANTID { get; set; }
        public string? DEVICE_ID { get; set; }
        public string? CANCEL_URL { get; set; }
    }

    // What the screen sees of a POS machine: name (MAP_TERMINAL_NAME) and EDC number (SECURITY_TOKEN); no merchant id
    public class PosDeviceDto
    {
        public int configId { get; set; }
        public string terminalName { get; set; } = "";
        public string? edcNo { get; set; }
        public string? deviceNo { get; set; }
        public int autoCancelMinutes { get; set; }
    }

    public class PosSettingsDto
    {
        public bool enabled { get; set; }
        public List<string> modes { get; set; } = new();     // CARD / UPI taken on the machine
        public bool allowManualCardEntry { get; set; }
    }

    // =====================================================
    // Requests
    // =====================================================
    public class PosInitiateRequest
    {
        public int sessionId { get; set; }
        public string purpose { get; set; } = "";        // CHECKIN / CHECKOUT / FOLIO / LAUNDRY / RESERVATION
        public int refId { get; set; }                   // booking id (laundry: laundry log id); 0 = new booking
        public string? refNo { get; set; }               // booking no; guest mobile for a new booking
        public string paymentMode { get; set; } = "CARD";
        public decimal amount { get; set; }
        public int deviceConfigId { get; set; }
        public string? customerName { get; set; }
        public string? mobileNo { get; set; }
        public string? patientId { get; set; }           // UMR no. when the guest is a patient attendant
    }

    public class PosStatusRequest
    {
        public int sessionId { get; set; }
        public long posTxnId { get; set; }
    }

    public class PosCloseRequest
    {
        public int sessionId { get; set; }
        public long posTxnId { get; set; }
        public string? reason { get; set; }
        public bool cancelledOnDevice { get; set; }      // staff confirms the machine shows cancelled / declined
    }

    // =====================================================
    // DGM_POS_TXN row
    // =====================================================
    public class PosTxnRow
    {
        public long POS_TXN_ID { get; set; }
        public string? TXN_NO { get; set; }
        public string PURPOSE { get; set; } = "";
        public int REF_ID { get; set; }
        public string? REF_NO { get; set; }
        public string PAYMENT_MODE { get; set; } = "";
        public string TXN_TYPE { get; set; } = "";
        public decimal AMOUNT { get; set; }
        public int DEVICE_CONFIG_ID { get; set; }
        public string? DEVICE_NO { get; set; }
        public string? TERMINAL_NM { get; set; }
        public string? CUSTOMER_NM { get; set; }
        public string? MOBILE_NO { get; set; }
        public string STATUS { get; set; } = "";
        public string? PL_TRANSACTION_ID { get; set; }
        public string? PL_REFERENCE_ID { get; set; }
        public string? RRN { get; set; }
        public string? CARD_MASKED { get; set; }
        public string? CARD_LAST4 { get; set; }
        public string? CARD_HOLDER_NM { get; set; }
        public decimal? PAID_AMOUNT { get; set; }
        public string? LAST_MESSAGE { get; set; }
        public int STATUS_CHECK_COUNT { get; set; }
        public string? CLAIM_STATUS { get; set; }
        public int? LINKED_BOOKING_ID { get; set; }
        public decimal? RECORDED_AMOUNT { get; set; }
        public string AMOUNT_MISMATCH { get; set; } = "N";
        public string? CLOSE_REASON { get; set; }
        public DateTime CREATE_DT { get; set; }
        public DateTime? SUCCESS_DT { get; set; }
        public DateTime? LINKED_DT { get; set; }
        public string? BOOKING_NO { get; set; }
    }

    // What the screen sees of a POS transaction
    public class PosTxnDto
    {
        public long posTxnId { get; set; }
        public string txnNo { get; set; } = "";
        public string purpose { get; set; } = "";
        public int refId { get; set; }
        public string? refNo { get; set; }
        public string paymentMode { get; set; } = "";
        public decimal amount { get; set; }
        public int deviceConfigId { get; set; }
        public string? terminalName { get; set; }
        public string status { get; set; } = "";        // INITIATED / PUSHED / UNKNOWN / SUCCESS / FAILED / PUSH_FAILED / CANCELLED
        public bool isPaid { get; set; }
        public bool isOpen { get; set; }                // waiting on the machine (device locked)
        public bool canCheckStatus { get; set; }
        public bool isSaved { get; set; }               // already used for a DGMS payment
        public string? rrn { get; set; }
        public string? cardMasked { get; set; }
        public string? cardLast4 { get; set; }
        public string? cardHolderName { get; set; }
        public string? message { get; set; }
        public int statusCheckCount { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? paidAt { get; set; }
        public int autoCancelMinutes { get; set; }

        // reconciliation list
        public string? customerName { get; set; }
        public string? bookingNo { get; set; }
        public decimal? recordedAmount { get; set; }
        public bool amountMismatch { get; set; }
        public string? closeReason { get; set; }
        public string? plTransactionId { get; set; }
    }

    public class PosApiLogRow
    {
        public long LOG_ID { get; set; }
        public string API_NAME { get; set; } = "";
        public string HTTP_METHOD { get; set; } = "";
        public string REQUEST_URL { get; set; } = "";
        public string? REQUEST_BODY { get; set; }
        public string? RESPONSE_BODY { get; set; }
        public int? HTTP_STATUS { get; set; }
        public string IS_SUCCESS { get; set; } = "N";
        public string? ERROR_MSG { get; set; }
        public int DURATION_MS { get; set; }
        public string? CORRELATION_ID { get; set; }
        public DateTime CREATE_DT { get; set; }
    }

    public class PosApiLogDto
    {
        public long logId { get; set; }
        public string apiName { get; set; } = "";
        public string requestUrl { get; set; } = "";
        public string? requestBody { get; set; }
        public string? responseBody { get; set; }
        public int? httpStatus { get; set; }
        public bool isSuccess { get; set; }
        public string? error { get; set; }
        public int durationMs { get; set; }
        public string? correlationId { get; set; }
        public DateTime createdAt { get; set; }
    }

    // A SUCCESS POS transaction reserved for one payment save
    public class PosClaim
    {
        public long PosTxnId { get; set; }
        public Guid Token { get; set; }
        public string TxnNo { get; set; } = "";
        public string Rrn { get; set; } = "";
        public string? CardLast4 { get; set; }
        public string? CardMasked { get; set; }
        public string? CardHolderName { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMode { get; set; } = "";
    }
}
