using System;

namespace Suvarna.DGMS.Api.Models
{
    // ==========================================
    // GST INVOICE REGISTER — one row
    // ==========================================
    public class GstInvoiceListItem
    {
        public int invoiceId { get; set; }
        public string invoiceNo { get; set; } = "";
        public DateTime invoiceDt { get; set; }
        public int bookingId { get; set; }

        public string customerName { get; set; } = "";
        public string? customerGstin { get; set; }

        public string sacCode { get; set; } = "";
        public decimal gstSlabPct { get; set; }

        public decimal baseAmount { get; set; }
        public decimal cgstAmount { get; set; }
        public decimal sgstAmount { get; set; }
        public decimal totalTaxAmount { get; set; }
        public decimal grossAmount { get; set; }

        public string invoiceStatus { get; set; } = "";   // ISSUED / PAID / CANCELLED
        public string returnType { get; set; } = "";      // B2B / B2C
    }

    // ==========================================
    // HSN / SAC SUMMARY — one row
    // ==========================================
    public class GstHsnSummary
    {
        public string sacCode { get; set; } = "";
        public string description { get; set; } = "";
        public decimal taxableAmount { get; set; }
        public decimal gstAmount { get; set; }
    }

    // ==========================================
    // GSTR-1 — B2B row (Table 4A)
    // ==========================================
    public class Gstr1B2BInvoice
    {
        public string invoiceNo { get; set; } = "";
        public DateTime invoiceDt { get; set; }
        public decimal invoiceValue { get; set; }
        public string sacCode { get; set; } = "";
        public decimal taxableValue { get; set; }
        public decimal gstRate { get; set; }
        public decimal cgst { get; set; }
        public decimal sgst { get; set; }
        public decimal igst { get; set; }
        public string? receiverGstin { get; set; }
        public string receiverName { get; set; } = "";
    }

    // ==========================================
    // GSTR-1 — B2C summary row (Table 7)
    // ==========================================
    public class Gstr1B2CSummary
    {
        public string stateCode { get; set; } = "";
        public string sacCode { get; set; } = "";
        public decimal gstRate { get; set; }
        public decimal taxableValue { get; set; }
        public decimal cgst { get; set; }
        public decimal sgst { get; set; }
        public decimal totalTax { get; set; }
        public int invoiceCount { get; set; }
    }

    // ==========================================
    // GSTR-3B SUMMARY (single row)
    // ==========================================
    public class Gstr3BSummary
    {
        public int invoiceCount { get; set; }
        public decimal totalTaxableValue { get; set; }
        public decimal totalCgst { get; set; }
        public decimal totalSgst { get; set; }
        public decimal totalIgst { get; set; }
        public decimal totalTax { get; set; }
        public decimal totalGross { get; set; }
    }

    // ==========================================
    // COMPLIANCE / FILING STATUS — one row
    // ==========================================
    public class GstComplianceItem
    {
        public int filingId { get; set; }
        public string returnType { get; set; } = "";      // GSTR1 / GSTR3B
        public int periodMonth { get; set; }
        public int periodYear { get; set; }
        public DateTime dueDt { get; set; }
        public DateTime? filedDt { get; set; }
        public string? arnNo { get; set; }
        public string filingStatus { get; set; } = "";     // FILED / OVERDUE / DUE_SOON / UPCOMING
        public int daysToDue { get; set; }
    }

    // ==========================================
    // ACTION RESULTS
    // ==========================================
    public class GstActionResult
    {
        public string status { get; set; } = "";
        public string message { get; set; } = "";
    }

    // ==========================================
    // REQUEST DTOs
    // ==========================================
    public class MarkGstInvoicePaidRequest
    {
        public int sessionId { get; set; }
        public int invoiceId { get; set; }
    }

    public class CancelGstInvoiceRequest
    {
        public int sessionId { get; set; }
        public int invoiceId { get; set; }
        public string cancelReason { get; set; } = "";
    }

    public class MarkGstFiledRequest
    {
        public int sessionId { get; set; }
        public int filingId { get; set; }
        public string? arnNo { get; set; }
    }
}
