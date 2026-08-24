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

        public List<BillingChargeLine> chargeLines { get; set; } = new();
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
        public string paymentMode { get; set; } = "CASH"; // CASH / CARD
        public decimal amount { get; set; }
        public string? paymentRef { get; set; }
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
