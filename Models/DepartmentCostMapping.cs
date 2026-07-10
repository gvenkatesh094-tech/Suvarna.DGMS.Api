namespace Suvarna.DGMS.Api.Models
{
    public class DepartmentCostMapping
    {
        public int deptMappingId { get; set; }

        public short grpId { get; set; }

        public short orgId { get; set; }

        public short locId { get; set; }
        public short sessionId { get; set; }


        public int propertyId { get; set; }

        public string propertyName { get; set; }

        public int accmTypeId { get; set; }

        public string accmTypeName { get; set; }

        public int occCatId { get; set; }

        public string occCatName { get; set; }
        public int deptId { get; set; }

        public string deptCd { get; set; }
        public string deptName { get; set; }

        public string costCenterCd { get; set; }

        public string glAccountCd { get; set; }

        public string remarks { get; set; }

        public string recordStatus { get; set; }

        public int createBy { get; set; }

        public string revCat { get; set; }

    }
}
