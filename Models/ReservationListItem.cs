namespace Suvarna.DGMS.Api.Models
{
    public class ReservationListItem
    {
        public int occCatId { get; set; }
        public string? occCatName { get; set; }

        public int? relationshipId { get; set; }
        public string? relationshipName { get; set; }

        public string? patientName { get; set; }
        public string? patientGender { get; set; }
        public string? umrNo { get; set; }
        public string? admnNo { get; set; }

        public string guestName { get; set; } = "";
        public string? gender { get; set; }
        public int? genderId { get; set; }
        public string? mobileNo { get; set; }
        public string? email { get; set; }
        public string? address { get; set; }


        public int bookingId { get; set; }
        public string bookingNo { get; set; } = "";               
        public int propertyId { get; set; }    
        public string? propertyName { get; set; }
        public int? roomId { get; set; }
        public string? roomNo { get; set; }
        public int? bedId { get; set; }
        public string? bedNo { get; set; }
        public int accmTypeId { get; set; }
        public string? accmTypeName { get; set; }
        public int tariffId { get; set; }
        public string? tariffName { get; set; }
        public DateTime? expectedCheckinDt { get; set; }
        public DateTime? expectedCheckoutDt { get; set; }
        public int nights { get; set; }
        public decimal? depositAmount { get; set; }
        public string bookingStatus { get; set; } = "";
        public DateTime? reservationDt { get; set; }
    }
}