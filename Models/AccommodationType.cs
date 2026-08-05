namespace Suvarna.DGMS.Api.Models
{
    public class AccomidationType
    {
        public string? mode { get; set; }   // I / U / D
        public int accmTypeId { get; set; }
        public int grpId { get; set; }
        public int orgId { get; set; }
        public int locationId { get; set; }

        public string accmTypeCode { get; set; }
        public string accmTypeName { get; set; }
        public int accmCategoryId { get; set; }
        
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
    public class AccommodationCategory
    {
        public int categoryId { get; set; }

        public string categoryCode { get; set; } = string.Empty;

        public string categoryName { get; set; } = string.Empty;

        public string description { get; set; } = string.Empty;

        public int displayOrder { get; set; }

        public string status { get; set; } = string.Empty;

        public int createdBy { get; set; }

        public DateTime? createdDate { get; set; }

        public int modifiedBy { get; set; }

        public DateTime? modifiedDate { get; set; }
    }

}