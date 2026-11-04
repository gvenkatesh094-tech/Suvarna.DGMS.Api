namespace Suvarna.DGMS.Api.Models
{
    // ==========================================
    // 28-Oct: HK Checklist Master (item library + room-type templates)
    // ==========================================

    public class HkChkItem
    {
        public int itemId { get; set; }
        public string itemCd { get; set; } = "";
        public string itemNm { get; set; } = "";
        public string? itemCategory { get; set; }
        public string photoRequired { get; set; } = "Y";
        public string isMandatory { get; set; } = "Y";
        public string? instructions { get; set; }
        public int? estMinutes { get; set; }
        public string recordStatus { get; set; } = "A";
        public int usedInTemplates { get; set; }
    }

    public class HkChkItemSaveRequest
    {
        public int sessionId { get; set; }
        public int itemId { get; set; }
        public string itemNm { get; set; } = "";
        public string? itemCategory { get; set; }
        public string photoRequired { get; set; } = "Y";
        public string isMandatory { get; set; } = "Y";
        public string? instructions { get; set; }
        public int? estMinutes { get; set; }
        public string recordStatus { get; set; } = "A";
    }

    public class HkChkTemplate
    {
        public int templateId { get; set; }
        public string templateCd { get; set; } = "";
        public string templateNm { get; set; } = "";
        public string taskType { get; set; } = "";
        public int? accmTypeId { get; set; }
        public string? accmTypeName { get; set; }
        public int? propertyId { get; set; }
        public string? propertyName { get; set; }
        public int slaMinutes { get; set; }
        public int slaWarnPct { get; set; }
        public int slaCriticalPct { get; set; }
        public string inspectionRequired { get; set; } = "Y";
        public string? remarks { get; set; }
        public string recordStatus { get; set; } = "A";
        public int itemCount { get; set; }
        public int photoCount { get; set; }
        public int mandatoryCount { get; set; }
    }

    public class HkChkTemplateItem
    {
        public int itemId { get; set; }
        public string itemCd { get; set; } = "";
        public string itemNm { get; set; } = "";
        public string? itemCategory { get; set; }
        public string? instructions { get; set; }
        public int sortOrder { get; set; }
        public string isMandatory { get; set; } = "Y";
        public string photoRequired { get; set; } = "Y";
        public string itemStatus { get; set; } = "A";
    }

    public class HkChkTemplateSaveRequest
    {
        public int sessionId { get; set; }
        public int templateId { get; set; }
        public string templateNm { get; set; } = "";
        public string taskType { get; set; } = "CHECKOUT_CLEAN";
        public int? accmTypeId { get; set; }
        public int? propertyId { get; set; }
        public int slaMinutes { get; set; } = 30;
        public int slaWarnPct { get; set; } = 70;
        public int slaCriticalPct { get; set; } = 90;
        public string inspectionRequired { get; set; } = "Y";
        public string? remarks { get; set; }
        public string recordStatus { get; set; } = "A";
        public List<HkChkTemplateItemInput> items { get; set; } = new();
    }

    public class HkChkTemplateItemInput
    {
        public int itemId { get; set; }
        public int sortOrder { get; set; }
        public string isMandatory { get; set; } = "Y";
        public string photoRequired { get; set; } = "Y";
    }

    public class HkChkRoomType
    {
        public int accmTypeId { get; set; }
        public string accmTypeName { get; set; } = "";
        public string? accmCategory { get; set; }
        public int roomCount { get; set; }
    }

    public class HkChkProperty
    {
        public int propertyId { get; set; }
        public string propertyName { get; set; } = "";
    }

    public class HkChkLookups
    {
        public List<HkChkRoomType> roomTypes { get; set; } = new();
        public List<HkChkProperty> properties { get; set; } = new();
    }

    public class HkChkSaveResult
    {
        public int id { get; set; }
        public string status { get; set; } = "";
        public string message { get; set; } = "";
    }
}
