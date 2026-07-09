namespace Suvarna.DGMS.Api.Models
{
    public class AccomidationType
    {
        public string? mode { get; set; }   // I / U / D
        public int accmTypeId { get; set; }
        public int grpId { get; set; }
        public int orgId { get; set; }
        public int locId { get; set; }

        public string accmTypeCode { get; set; }
        public string accmTypeName { get; set; }
        public string accmCategory { get; set; }

        public int maxOccupants { get; set; }

        public string? amenities { get; set; }

        public decimal gst { get; set; }

        public string sacCode { get; set; }

        public string isGstInclusive { get; set; }

        public string isFoodIncluded { get; set; }

        public string isAc { get; set; }

        public int sortOrder { get; set; }

        public string? remarks { get; set; }

        public string? iconCd { get; set; }

        public string status { get; set; }

        public int userId { get; set; }
        
        public decimal defaultRate { get; set; }
        public int  sessionId { get; set; }


    }
//    public class deleteAccomidationType
//    {
//        public int accmTypeId { get; set; }
//<<<<<<< HEAD
//        public int userId { get; set; }
//=======

//        public string? accmTypeCode { get; set; }

//        public string? accmTypeName { get; set; }
//        public decimal? gst { get; set; }

//        public decimal? roomPrice { get; set; }
//>>>>>>> 42f3f470f5e862c98cb3c8c8ef76e187668b4ad6
//    }
  
}