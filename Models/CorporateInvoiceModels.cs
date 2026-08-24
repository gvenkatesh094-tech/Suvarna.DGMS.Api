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
        public List<CorpInvoiceBookingLine> bookings { get; set; } = new();
    }
}
