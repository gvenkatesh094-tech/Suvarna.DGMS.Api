using System;

namespace Suvarna.DGMS.Api.Models
{
    public class CheckoutBooking
    {
        public int bookingId { get; set; }

        public string bookingNo { get; set; } = "";

        public string guestName { get; set; } = "";

        public string mobileNo { get; set; } = "";

        public int roomId { get; set; }

        public string roomNo { get; set; } = "";

        public string roomName { get; set; } = "";

        public DateTime checkinDate { get; set; }

        public DateTime expectedCheckoutDate { get; set; }

        public int stayDays { get; set; }

        public decimal totalBill { get; set; }

        public decimal deposit { get; set; }

        public decimal paidAmount { get; set; }

        public decimal balanceAmount { get; set; }

        public string bookingStatus { get; set; } = "";

        public string checkoutStatus { get; set; } = "";
    }
}