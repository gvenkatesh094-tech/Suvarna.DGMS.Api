namespace Suvarna.DGMS.Api.Models
{
    // -----------------------------------------------------------------
    // Dropdown item — "Select Guest"
    // -----------------------------------------------------------------
    public class BillingBookingLookup
    {
        public int bookingId { get; set; }
        public string bookingRef { get; set; } = "";
        public string guestNm { get; set; } = "";
        public string roomNo { get; set; } = "";
        public int propertyId { get; set; }

        // NEW (Round B): checked-out guests are listed too (final bill / tax invoice)
        public string bookingStatus { get; set; } = "CHECKEDIN";
        public string? gstInvoiceNo { get; set; }
        public decimal balanceAmt { get; set; }
        public string? billStatus { get; set; }   // 28-Oct: OPEN / PARTIALLY_SETTLED / SETTLED / TRANSFERRED
    }

    // -----------------------------------------------------------------
    // Dropdown item — "Add Charge -> Service"
    // -----------------------------------------------------------------
    public class BillingServiceLookup
    {
        public int serviceId { get; set; }
        public string serviceNm { get; set; } = "";
        public decimal rate { get; set; }
        public decimal gstPct { get; set; }
        public string sacCode { get; set; } = "";
    }

    // -----------------------------------------------------------------
    // Charge line — Charge Lines tab table row
    // -----------------------------------------------------------------
    public class BillingChargeLine
    {
        public int chargeId { get; set; }
        public DateTime chargeDt { get; set; }
        public string description { get; set; } = "";
        public string sacCode { get; set; } = "";
        public decimal baseAmt { get; set; }
        public decimal gstPct { get; set; }
        public decimal totalAmt { get; set; }
        public string status { get; set; } = "";

        // FIX (missing): 'MANUAL' / 'LAUNDRY' — GuestFolioBillingRepository.GetFolio
        // sets this from the new CHARGE_SOURCE column returned by
        // PR_DGMS_GET_BILLING_FOLIO. Without this property the object
        // initializer there (`chargeSource = ...`) won't compile.
        // The UI uses it to hide the Cancel button on laundry-sourced
        // charges and point staff to the Laundry Log screen instead.
        public string chargeSource { get; set; } = "MANUAL";

        // NEW (Round B): ACCOMMODATION / SERVICE / PENALTY - only SERVICE lines can be cancelled
        public string chargeType { get; set; } = "SERVICE";

        // NEW 04-Nov: who posted / paid / cancelled the line, and when (null until the 04-Nov SQL is run)
        public DateTime? postedDt { get; set; }
        public string? postedByNm { get; set; }
        public decimal paidAmt { get; set; }
        public string? payStatus { get; set; }          // PAID / PART_PAID / UNPAID / COMPANY / CANCELLED
        public int? paymentId { get; set; }             // receipt PAY-<id>
        public DateTime? paidDt { get; set; }
        public string? paidByNm { get; set; }
        public string? paymentMode { get; set; }
        public DateTime? cancelledDt { get; set; }
        public string? cancelledByNm { get; set; }
        public string? cancelReason { get; set; }
    }

    // NEW 04-Nov: one payment (receipt) of the stay
    public class BillingPaymentLine
    {
        public int paymentId { get; set; }
        public string receiptNo { get; set; } = "";
        public DateTime? paymentDt { get; set; }
        public DateTime? createDt { get; set; }
        public string paymentMode { get; set; } = "";
        public decimal amount { get; set; }
        public string? paymentRef { get; set; }
        public string? status { get; set; }
        public string? remarks { get; set; }
        public string? collectedByNm { get; set; }
        public int servicesPaid { get; set; }
    }

    // NEW 04-Nov: payment receipt (print / reprint) - header + lines
    public class FolioReceipt
    {
        public int paymentId { get; set; }
        public string receiptNo { get; set; } = "";
        public DateTime? paymentDt { get; set; }
        public string paymentMode { get; set; } = "";
        public decimal amount { get; set; }
        public string? paymentRef { get; set; }
        public string? paymentStatus { get; set; }
        public string? remarks { get; set; }
        public string? collectedByNm { get; set; }
        public string bookingNo { get; set; } = "";
        public string guestNm { get; set; } = "";
        public string? mobileNo { get; set; }
        public string? roomNo { get; set; }
        public DateTime? checkinDt { get; set; }
        public string? billNo { get; set; }
        public decimal billTotalAmt { get; set; }
        public decimal paidTillAmt { get; set; }
        public decimal balanceNowAmt { get; set; }
        public string billToType { get; set; } = "GUEST";
        public string? corpNm { get; set; }
        public string? propertyNm { get; set; }
        public string? propertyAddress { get; set; }
        public string? propertyPhone { get; set; }
        public string? propertyGstin { get; set; }
        public List<FolioReceiptLine> lines { get; set; } = new();
    }

    public class FolioReceiptLine
    {
        public int chargeId { get; set; }
        public DateTime? chargeDt { get; set; }
        public string description { get; set; } = "";
        public string? sacCode { get; set; }
        public decimal qty { get; set; }
        public decimal unitRate { get; set; }
        public decimal baseAmt { get; set; }
        public decimal gstPct { get; set; }
        public decimal gstAmt { get; set; }
        public decimal lineAmt { get; set; }
        public decimal paidAmt { get; set; }
    }

    // -----------------------------------------------------------------
    // Full folio — guest details + bill summary + deposit + IP info
    // -----------------------------------------------------------------
    public class BillingFolio
    {
        public int bookingId { get; set; }
        public string bookingRef { get; set; } = "";
        public string guestNm { get; set; } = "";
        public string roomNo { get; set; } = "";
        public DateTime? checkinDt { get; set; }
        public DateTime? checkoutDt { get; set; }
        public string categoryNm { get; set; } = "";
        public int nights { get; set; }
        public string mobileNo { get; set; } = "";

        public int billId { get; set; }
        public string billNo { get; set; } = "";
        public decimal totalAccmAmt { get; set; }
        public decimal totalSvcAmt { get; set; }
        public decimal totalBaseAmt { get; set; }
        public decimal totalCgstAmt { get; set; }
        public decimal totalSgstAmt { get; set; }
        public decimal totalGstAmt { get; set; }
        public decimal totalGrossAmt { get; set; }
        public decimal netPayableAmt { get; set; }
        public decimal depositAdjustedAmt { get; set; }
        public decimal totalPaidAmt { get; set; }
        public decimal balanceAmt { get; set; }
        public string billStatus { get; set; } = "";

        public decimal depositAmt { get; set; }
        public decimal depositBalance { get; set; }
        public string depositStatus { get; set; } = "";

        public string? umrNo { get; set; }
        public string? admnNo { get; set; }
        public int? patientId { get; set; }
        public int? admnId { get; set; }
        public string? wardNm { get; set; }
        public string? transferStatus { get; set; }
        public string? gstInvoiceNo { get; set; }
        public DateTime? gstInvoiceDt { get; set; }

        // NEW (Round B)
        public string bookingStatus { get; set; } = "CHECKEDIN";
        public decimal discountAmt { get; set; }
        public string billToType { get; set; } = "GUEST";
        public string? corpNm { get; set; }
        public int? gstInvoiceId { get; set; }
        // NEW 02-Nov: company stay split (null when the 02-Nov SQL is not run yet)
        public string? corpBillingScope { get; set; }
        public decimal? guestDueAmt { get; set; }
        public decimal? corpDueAmt { get; set; }

        public List<BillingChargeLine> chargeLines { get; set; } = new();
        public List<BillingPaymentLine> payments { get; set; } = new();   // NEW 04-Nov
    }

    // -----------------------------------------------------------------
    // NEW (Round B): printable GST tax invoice
    // -----------------------------------------------------------------
    public class GstInvoicePrint
    {
        public int invoiceId { get; set; }
        public string invoiceNo { get; set; } = "";
        public DateTime? invoiceDt { get; set; }          // UTC
        public string invoiceStatus { get; set; } = "";

        public string propertyNm { get; set; } = "";
        public string? propertyAddress { get; set; }
        public string? propertyCity { get; set; }
        public string? propertyState { get; set; }
        public string? propertyPin { get; set; }
        public string? propertyPhone { get; set; }
        public string? propertyEmail { get; set; }
        public string? supplierGstin { get; set; }
        public string? placeOfSupply { get; set; }

        public string customerNm { get; set; } = "";
        public string? customerAddress { get; set; }
        public string? customerGstin { get; set; }
        public string? customerStateCd { get; set; }

        public string bookingNo { get; set; } = "";
        public string roomNo { get; set; } = "";
        public DateTime? checkinDt { get; set; }          // UTC
        public DateTime? checkoutDt { get; set; }         // UTC

        public decimal baseAmt { get; set; }
        public decimal cgstAmt { get; set; }
        public decimal sgstAmt { get; set; }
        public decimal igstAmt { get; set; }
        public decimal totalTaxAmt { get; set; }
        public decimal discountAmt { get; set; }
        public decimal grossAmt { get; set; }
        public decimal roundingAdj { get; set; }
        public decimal roundedAmt { get; set; }
        public decimal paidAmt { get; set; }
        public decimal depositAdjustedAmt { get; set; }
        public decimal balanceAmt { get; set; }
        public string billToType { get; set; } = "GUEST";

        public List<GstInvoicePrintLine> lines { get; set; } = new();
    }

    public class GstInvoicePrintLine
    {
        public int lineNo { get; set; }
        public string sacCode { get; set; } = "";
        public string description { get; set; } = "";
        public string? uom { get; set; }
        public decimal qty { get; set; }
        public decimal unitRate { get; set; }
        public decimal baseAmt { get; set; }
        public decimal gstPct { get; set; }
        public decimal cgstAmt { get; set; }
        public decimal sgstAmt { get; set; }
        public decimal igstAmt { get; set; }
        public decimal lineTotal { get; set; }
    }

    // -----------------------------------------------------------------
    // Request DTOs
    // -----------------------------------------------------------------
    public class AddChargeRequest
    {
        public int sessionId { get; set; }
        public int bookingId { get; set; }
        public int serviceId { get; set; }
        public decimal qty { get; set; } = 1;
    }

    public class CancelChargeRequest
    {
        public int sessionId { get; set; }
        public int chargeId { get; set; }
        public string? reason { get; set; }
    }

    public class UpdateChargeRequest
    {
        public int sessionId { get; set; }
        public int chargeId { get; set; }
        public DateTime chargeDt { get; set; }
        public string description { get; set; } = "";
        public string sacCode { get; set; } = "";
        public decimal baseAmt { get; set; }
        public decimal gstPct { get; set; }
        public string status { get; set; } = "POSTED";
    }

    public class CollectPaymentRequest
    {
        public int sessionId { get; set; }
        public int bookingId { get; set; }
        public string paymentMode { get; set; } = "CASH"; // CASH / CARD / UPI / BANK_TRANSFER / CHEQUE
        public decimal amount { get; set; }
        public string? paymentRef { get; set; }

        // NEW: SUCCESS POS payment (api/pos-payment); the RRN is taken from it
        public long? posTxnId { get; set; }
    }

    public class TransferToIpRequest
    {
        public int sessionId { get; set; }
        public int bookingId { get; set; }
    }

    public class GenerateInvoiceRequest
    {
        public int sessionId { get; set; }
        public int bookingId { get; set; }
    }

    // -----------------------------------------------------------------
    // Result DTOs
    // -----------------------------------------------------------------
    public class ChargeResult
    {
        public int chargeId { get; set; }
        public string status { get; set; } = "";
        public string message { get; set; } = "";
    }

    public class PaymentResult
    {
        public int paymentId { get; set; }
        public string status { get; set; } = "";
        public string message { get; set; } = "";
    }

    public class TransferResult
    {
        public string status { get; set; } = "";
        public string message { get; set; } = "";
    }

    public class InvoiceResult
    {
        public int invoiceId { get; set; }
        public string invoiceNo { get; set; } = "";
        public string status { get; set; } = "";
        public string message { get; set; } = "";
    }
}