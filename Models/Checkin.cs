namespace Suvarna.DGMS.Api.Models
{
    public class Checkin
    {
        public int sessionId { get; set; } 

        public int? existingBookingId { get; set; }

        public int propertyId { get; set; }
        public int roomId { get; set; }
        public int? bedId { get; set; }
        public int tariffId { get; set; }
        public int occCatId { get; set; }

        public string guestName { get; set; } = "";
        public string? genderCd { get; set; }
        public DateTime? dob { get; set; }
        public string mobileNo { get; set; } = "";
        public string? emailId { get; set; }

        public string idTypeCd { get; set; } = "";
        public string idNumberMasked { get; set; } = "";
        public string? photoPath { get; set; }

        public string? guestGstin { get; set; }
        public string? guestStateCd { get; set; }

        public string? vehicleNo { get; set; }
        public string? purpose { get; set; }

        public string? addressLine1 { get; set; }
        public string? city { get; set; }
        public string? stateCd { get; set; }

        public string? emergencyName { get; set; }
        public string? emergencyPhone { get; set; }

        public int? patientId { get; set; }
        public string? umrNo { get; set; }
        public int? admnId { get; set; }
        public string? admnNo { get; set; }
        public string? relationshipCd { get; set; }

        public int? corpAgreementId { get; set; }
        public string? corpPoRef { get; set; }

        public DateTime expectedCheckoutDt { get; set; }

        public decimal depositAmount { get; set; }
        public string? depositPaymentMode { get; set; }
        public string? depositRef { get; set; }
    }

    public class CheckinResult
    {
        public int bookingId { get; set; }
        public string bookingNo { get; set; } = "";

        public int billId { get; set; }
        public string billNo { get; set; } = "";

        public string status { get; set; } = "";
        public string message { get; set; } = "";
    }
}