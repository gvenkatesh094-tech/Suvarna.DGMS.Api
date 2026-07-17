using System;

namespace Suvarna.DGMS.Api.Models
{
    public class CheckoutBooking
    {
        public int bookingId { get; set; }

        public string bookingNo { get; set; } = string.Empty;

        public string guestName { get; set; } = string.Empty;

        public string mobileNo { get; set; } = string.Empty;

        public int propertyId { get; set; }

        public string propertyName { get; set; } = string.Empty;

        public int roomId { get; set; }

        public string roomNo { get; set; } = string.Empty;

        public int? bedId { get; set; }

        public string? bedNo { get; set; }

        public int tariffId { get; set; }

        public string tariffName { get; set; } = string.Empty;

        public int occCatId { get; set; }

        public string occCatName { get; set; } = string.Empty;

        public DateTime checkinDate { get; set; }

        public DateTime expectedCheckoutDate { get; set; }

        public int totalNights { get; set; }

        public decimal roomCharges { get; set; }

        public decimal serviceCharges { get; set; }

        public decimal grossAmount { get; set; }

        public decimal depositAmount { get; set; }

        public decimal balanceAmount { get; set; }

        public string bookingStatus { get; set; } = string.Empty;

        public string? patientName { get; set; }

        public string? uhid { get; set; }

        public string? wardName { get; set; }

        public string? corporateName { get; set; }
    }
}