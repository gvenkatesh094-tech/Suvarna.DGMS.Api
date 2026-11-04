namespace Suvarna.DGMS.Api.Models
{
    public class CorpInvoiceRow
    {
        public int invoiceId { get; set; }
        public string invoiceNo { get; set; } = "";
        public string corpNm { get; set; } = "";
        public string period { get; set; } = "";
        public int totalBookings { get; set; }
        public decimal totalBaseAmt { get; set; }
        public decimal totalGstAmt { get; set; }
        public decimal netPayableAmt { get; set; }
        public DateTime? dueDt { get; set; }
        public string status { get; set; } = "";

        // NEW (Round B)
        public DateTime? paymentDt { get; set; }
        public string? paymentRef { get; set; }
        public string isOverdue { get; set; } = "N";
        public int agreementId { get; set; }

        // NEW 04-Nov: audit - who created / collected / cancelled, and when (null until the 04-Nov SQL is run)
        public DateTime? createdDt { get; set; }
        public string? createdByNm { get; set; }
        public DateTime? paidEntryDt { get; set; }      // when the payment was entered
        public string? paidByNm { get; set; }
        public decimal? paidAmt { get; set; }
        public decimal? tdsAmt { get; set; }
        public string? paymentMode { get; set; }
        public DateTime? cancelledDt { get; set; }
        public string? cancelledByNm { get; set; }
        public string? cancelReason { get; set; }
    }

    public class CorpAgreementLookup
    {
        public int agreementId { get; set; }
        public string corpNm { get; set; } = "";
        public string agreementNo { get; set; } = "";
        public int creditDays { get; set; }
        public decimal discountPct { get; set; }
    }

    public class GenerateCorpInvoiceRequest
    {
        public int sessionId { get; set; }
        public int agreementId { get; set; }
        public DateTime periodFrom { get; set; }
        public DateTime periodTo { get; set; }
    }

    public class CorpInvoiceGenerateResult
    {
        public int invoiceId { get; set; }
        public string invoiceNo { get; set; } = "";
        public string status { get; set; } = "";
        public string message { get; set; } = "";
    }

    public class CorpInvoiceBookingLine
    {
        public string bookingNo { get; set; } = "";
        public string guestNm { get; set; } = "";
        public string roomNo { get; set; } = "";
        public DateTime billDt { get; set; }
        public decimal totalGrossAmt { get; set; }

        // NEW (Round B)
        public string? gstInvoiceNo { get; set; }
        public DateTime? checkoutDt { get; set; }
        public decimal baseAmt { get; set; }
        public decimal gstAmt { get; set; }

        // NEW 02-Nov: what the stay was made of (incl. GST)
        public decimal roomGrossAmt { get; set; }
        public decimal serviceGrossAmt { get; set; }
        public decimal penaltyAmt { get; set; }
    }

    public class CorpInvoiceDetail
    {
        public int invoiceId { get; set; }
        public string invoiceNo { get; set; } = "";
        public string corpNm { get; set; } = "";
        public string? corpGstin { get; set; }
        public DateTime billingPeriodFrom { get; set; }
        public DateTime billingPeriodTo { get; set; }
        public int totalBookings { get; set; }
        public decimal totalBaseAmt { get; set; }
        public decimal totalGstAmt { get; set; }
        public decimal totalGrossAmt { get; set; }
        public decimal discountAmt { get; set; }
        public decimal netPayableAmt { get; set; }
        public DateTime dueDt { get; set; }
        public string status { get; set; } = "";

        // NEW (Round B)
        public string? corpAddress { get; set; }
        public DateTime? invoiceDt { get; set; }
        public DateTime? paymentDt { get; set; }
        public string? paymentRef { get; set; }
        public string? remarks { get; set; }

        // NEW 04-Nov: audit - who created / collected / cancelled, and when (null until the 04-Nov SQL is run)
        public DateTime? createdDt { get; set; }
        public string? createdByNm { get; set; }
        public DateTime? paidEntryDt { get; set; }      // when the payment was entered
        public string? paidByNm { get; set; }
        public decimal? paidAmt { get; set; }
        public decimal? tdsAmt { get; set; }
        public string? paymentMode { get; set; }
        public DateTime? cancelledDt { get; set; }
        public string? cancelledByNm { get; set; }
        public string? cancelReason { get; set; }
        // NEW 04-Nov: payment receipt
        public string? receiptNo { get; set; }
        public string? propertyNm { get; set; }
        public string? propertyAddress { get; set; }
        public string? propertyGstin { get; set; }
        public string? propertyPhone { get; set; }

        public List<CorpInvoiceBookingLine> bookings { get; set; } = new();
    }

    // NEW (Round B): company payment of a corporate invoice (amount received + TDS = invoice amount)
    public class CorpInvoicePaymentRequest
    {
        public int sessionId { get; set; }
        public int invoiceId { get; set; }
        public decimal amount { get; set; }
        public decimal tdsAmount { get; set; }
        public string paymentMode { get; set; } = "BANK_TRANSFER";
        public string? paymentRef { get; set; }
        public DateTime? paymentDt { get; set; }
        public string? remarks { get; set; }
    }

    // NEW (Round B)
    public class CorpInvoiceCancelRequest
    {
        public int sessionId { get; set; }
        public int invoiceId { get; set; }
        public string? reason { get; set; }
    }

    public class CorpInvoiceActionResult
    {
        public string status { get; set; } = "";
        public string message { get; set; } = "";
    }
}
