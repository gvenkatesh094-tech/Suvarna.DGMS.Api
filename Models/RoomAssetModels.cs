namespace Suvarna.DGMS.Api.Models
{
    public class RoomAssetRow
    {
        public int assetId { get; set; }
        public string assetNo { get; set; } = "";
        public int roomId { get; set; }
        public string roomNo { get; set; } = "";
        public string assetType { get; set; } = "";
        public string assetNm { get; set; } = "";
        public string? brand { get; set; }
        public string? modelNo { get; set; }
        public string? serialNo { get; set; }
        public DateTime? purchaseDt { get; set; }
        public decimal? purchaseCostAmt { get; set; }
        public DateTime? warrantyExpiryDt { get; set; }
        public DateTime? lastMaintDt { get; set; }
        public DateTime? nextPmDt { get; set; }
        public int defectCount { get; set; }
        public decimal totalRepairCostAmt { get; set; }
        public string assetStatus { get; set; } = "";
        public string? remarks { get; set; }
        public bool isWarrantyExpired { get; set; }
    }

    public class AssetRoomLookup
    {
        public int roomId { get; set; }
        public string roomNo { get; set; } = "";
        public string accmTypeNm { get; set; } = "";
    }

    public class SaveRoomAssetRequest
    {
        public int sessionId { get; set; }
        public int? assetId { get; set; }       // null/0 = add new
        public string assetNo { get; set; } = "";
        public int roomId { get; set; }
        public string assetType { get; set; } = "";
        public string? brand { get; set; }
        public string? modelNo { get; set; }
        public string? serialNo { get; set; }
        public DateTime? purchaseDt { get; set; }
        public decimal? purchaseCostAmt { get; set; }
        public DateTime? warrantyExpiryDt { get; set; }
        public string assetStatus { get; set; } = "ACTIVE";
        public string? remarks { get; set; }
    }

    public class SaveAssetResult
    {
        public int assetId { get; set; }
        public string status { get; set; } = "";
        public string message { get; set; } = "";
    }

    public class LogAssetRepairRequest
    {
        public int sessionId { get; set; }
        public int assetId { get; set; }
        public decimal repairCost { get; set; }
        public string newStatus { get; set; } = "";
        public string? remarks { get; set; }
    }

    public class AssetActionResult
    {
        public string status { get; set; } = "";
        public string message { get; set; } = "";
    }

    public class AssetDuePmRow
    {
        public int assetId { get; set; }
        public string assetNo { get; set; } = "";
        public string assetNm { get; set; } = "";
        public string roomNo { get; set; } = "";
        public DateTime? nextPmDt { get; set; }
    }
}
