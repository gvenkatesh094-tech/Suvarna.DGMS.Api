namespace Suvarna.DGMS.Api.Models
{
    // =========================================
    // USER PROPERTY ACCESS (permission screen)
    // =========================================

    public class AccessUserModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int? HomeLocId { get; set; }
        public bool IsAdmin { get; set; }
        public int PropertyCount { get; set; }
    }

    public class AccessPropertyModel
    {
        public int PropertyId { get; set; }
        public string PropertyCd { get; set; } = string.Empty;
        public string PropertyNm { get; set; } = string.Empty;
        public string? PropertyTypeCd { get; set; }
        public string? City { get; set; }
        public int LocId { get; set; }
    }

    public class UserPropertyAccessItem
    {
        public int PropertyId { get; set; }
        public int LocId { get; set; }
        public bool IsDefault { get; set; }
        public string PropertyCd { get; set; } = string.Empty;
        public string PropertyNm { get; set; } = string.Empty;
    }

    public class UserPropertyAccessModel
    {
        public int UserId { get; set; }
        public bool IsAdmin { get; set; }
        public string? Remarks { get; set; }
        public DateTime? ModifyDt { get; set; }
        public List<UserPropertyAccessItem> Properties { get; set; } = new();
    }

    public class SaveUserPropertyAccessRequest
    {
        public int UserId { get; set; }
        public bool IsAdmin { get; set; }
        public List<int> PropertyIds { get; set; } = new();
        public int? DefaultPropertyId { get; set; }
        public string? Remarks { get; set; }
    }
}
