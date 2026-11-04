using System;
using System.Collections.Generic;

namespace Suvarna.DGMS.Api.Models
{
    public class CheckoutDetails
    {
        //=========================================
        // Booking Information
        //=========================================

        public int bookingId { get; set; }

        public string bookingNo { get; set; } = string.Empty;

        public string bookingStatus { get; set; } = string.Empty;

        public int propertyId { get; set; }

        public string propertyName { get; set; } = string.Empty;

        public int buildingId { get; set; }

        public string buildingName { get; set; } = string.Empty;

        public int floorId { get; set; }

        public string floorName { get; set; } = string.Empty;

        public int roomId { get; set; }

        public string roomNo { get; set; } = string.Empty;

        public string roomName { get; set; } = string.Empty;

        public int? bedId { get; set; }

        public string? bedNo { get; set; }

        //=========================================
        // Guest Information
        //=========================================

        public string guestName { get; set; } = string.Empty;

        public string mobileNo { get; set; } = string.Empty;

        public string? emailId { get; set; }

        public int occCatId { get; set; }

        public string occCatName { get; set; } = string.Empty;

        public string? patientName { get; set; }

        public string? uhid { get; set; }

        public string? wardName { get; set; }

        public string? corporateName { get; set; }

        //=========================================
        // Stay Details
        //=========================================

        public DateTime checkinDate { get; set; }

        public DateTime expectedCheckoutDate { get; set; }

        public DateTime checkoutDate { get; set; }

        public int totalNights { get; set; }

        //=========================================
        // Tariff
        //=========================================

        public int tariffId { get; set; }

        public string tariffCode { get; set; } = string.Empty;

        public string tariffName { get; set; } = string.Empty;

        public decimal dailyRate { get; set; }

        public decimal hourlyRate { get; set; }

        //=========================================
        // Charges
        //=========================================

        public decimal roomCharges { get; set; }

        public decimal serviceCharges { get; set; }

        public decimal damageCharges { get; set; }

        public decimal discountAmount { get; set; }

        public decimal taxableAmount { get; set; }

        public decimal gstPercentage { get; set; }

        public decimal gstAmount { get; set; }

        public decimal grossAmount { get; set; }

        //=========================================
        // Deposit
        //=========================================

        public decimal depositCollected { get; set; }

        public decimal depositAdjusted { get; set; }

        public decimal refundAmount { get; set; }

        //=========================================
        // Final Amount
        //=========================================

        public decimal netPayable { get; set; }

        public decimal amountPaid { get; set; }

        public decimal balanceAmount { get; set; }

        //=========================================
        // Bill Information
        //=========================================

        public int billId { get; set; }

        public string billNo { get; set; } = string.Empty;

        public string billStatus { get; set; } = string.Empty;
        public decimal depositBalance { get; set; }

        
        public decimal roomGstAmount { get; set; }

        // NEW: room rent paid at check-in that is more than the final bill (early checkout)
        public decimal excessPaidRefund { get; set; }

        // NEW: corporate booking (used to offer "Bill to company")
        public int? corpAgreementId { get; set; }

        // NEW 02-Nov: bill to company. GUEST / CORPORATE (from the bill), what the company pays
        // (ALL = room + services, ROOM_ONLY = room only), the guest's part and the company's part.
        public string billToType { get; set; } = "GUEST";
        public string? corpBillingScope { get; set; }
        public string? agreementNo { get; set; }
        public string? corpPoRef { get; set; }
        public decimal guestShareAmount { get; set; }
        public decimal corpBillAmount { get; set; }
        public decimal serviceGstAmount { get; set; }

        //=========================================
        // Services
        //=========================================

        public List<CheckoutServiceItem> services { get; set; }
            = new List<CheckoutServiceItem>();
        public List<CheckoutOpenLaundry> openLaundry { get; set; } = new();
    }
    public class CheckoutOpenLaundry
    {
        public int laundryLogId { get; set; }
        public string slipNo { get; set; } = "";
        public string laundryType { get; set; } = "";
        public int itemCount { get; set; }
        public string status { get; set; } = "";     // PENDING / IN_PROGRESS
        public DateTime? pickupDt { get; set; }
        public DateTime createDt { get; set; }
    }

    public class CheckoutServiceItem
    {
        public int serviceId { get; set; }

        public string serviceCode { get; set; } = string.Empty;

        public string serviceName { get; set; } = string.Empty;

        public decimal quantity { get; set; }

        public decimal rate { get; set; }

        public decimal amount { get; set; }
    }
}