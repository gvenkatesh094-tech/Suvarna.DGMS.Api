namespace Suvarna.DGMS.Api.Models
{
    public class Reservation
    {
       

        public int? bookingId { get; set; }
        public int sessionId { get; set; }
        public int propertyId { get; set; }
        public int roomId { get; set; }
        public int? bedId { get; set; }
        public int accmTypeId { get; set; }
        public int tariffId { get; set; }

        public int occCatId { get; set; }
        public string? occCatCd { get; set; }

        public string guestName { get; set; } = "";

        public string? genderCd { get; set; }
        public int? genderId { get; set; }

        public string mobileNo { get; set; } = "";
        public string? emailId { get; set; }
        public string? address { get; set; }

        public int? patientId { get; set; }
        public string? umrNo { get; set; }
        public string? admnNo { get; set; }

        public string? relationshipCd { get; set; }
        public int? relationshipId { get; set; }

        // ==========================================
        // CORPORATE
        // ==========================================

        public int? corpAgreementId { get; set; }

        public string? corpPoRef { get; set; }

        // ==========================================
        // RESERVATION DATES
        // ==========================================

        public DateTime expectedCheckinDt { get; set; }

        public DateTime expectedCheckoutDt { get; set; }

     
        // ==========================================
        // REMARKS
        // ==========================================

        public string? remarks { get; set; }
    }
}