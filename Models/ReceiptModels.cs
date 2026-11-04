namespace Suvarna.DGMS.Api.Models
{
    /// <summary>
    /// NEW 04-Nov: standard receipt print. The screen calls POST api/receipts/print just before it prints;
    /// the API logs the print and returns copy no, printed by / on, property header and paper size.
    /// </summary>
    public class ReceiptPrintRequest
    {
        public int SessionId { get; set; }
        public string DocType { get; set; } = "";      // CHECKIN, ADVANCE, FOLIO, LAUNDRY, REFUND, VISITOR, FINAL_BILL, TAX_INVOICE, CORP_RECEIPT, BILL_DETAILS
        public string DocNo { get; set; } = "";
        public string SizeGroup { get; set; } = "COUNTER"; // COUNTER (80 mm by default) / OFFICE (A4 by default)
        public int? PropertyId { get; set; }
        public bool Reprint { get; set; }
    }

    public class ReceiptPrintInfo
    {
        public int CopyNo { get; set; }
        public string PaperSize { get; set; } = "80MM";
        public DateTime PrintedDt { get; set; }
        public string? PrintedByNm { get; set; }
        public int? PrintedById { get; set; }
        public string? PropertyNm { get; set; }
        public string? PropertyAddress { get; set; }
        public string? PropertyPhone { get; set; }
        public string? PropertyGstin { get; set; }
    }
}
