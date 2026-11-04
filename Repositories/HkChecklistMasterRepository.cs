using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;
using System.Text.Json;

namespace Suvarna.DGMS.Api.Repositories
{
    // 28-Oct: HK Checklist Master - item library + room-type templates (DGMS.PR_DGMS_HK_CHK_*)
    public class HkChecklistMasterRepository
    {
        private readonly string _conn;

        public HkChecklistMasterRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        // ==========================================
        // LOOKUPS (room types + properties)
        // ==========================================

        public HkChkLookups GetLookups(string sessionId)
        {
            var result = new HkChkLookups();
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_HK_CHK_LOOKUPS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

                conn.Open();
                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.roomTypes.Add(new HkChkRoomType
                    {
                        accmTypeId = GetInt(dr, "accmTypeId"),
                        accmTypeName = GetString(dr, "accmTypeName") ?? "",
                        accmCategory = GetString(dr, "accmCategory"),
                        roomCount = GetInt(dr, "roomCount")
                    });
                }
                if (dr.NextResult())
                {
                    while (dr.Read())
                    {
                        result.properties.Add(new HkChkProperty
                        {
                            propertyId = GetInt(dr, "propertyId"),
                            propertyName = GetString(dr, "propertyName") ?? ""
                        });
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching checklist master lookups.", ex);
            }
        }

        // ==========================================
        // ITEM LIBRARY
        // ==========================================

        public List<HkChkItem> GetItems(string sessionId)
        {
            var list = new List<HkChkItem>();
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_HK_CHK_ITEM_LIST", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

                conn.Open();
                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new HkChkItem
                    {
                        itemId = GetInt(dr, "itemId"),
                        itemCd = GetString(dr, "itemCd") ?? "",
                        itemNm = GetString(dr, "itemNm") ?? "",
                        itemCategory = GetString(dr, "itemCategory"),
                        photoRequired = GetString(dr, "photoRequired") ?? "Y",
                        isMandatory = GetString(dr, "isMandatory") ?? "Y",
                        instructions = GetString(dr, "instructions"),
                        estMinutes = GetNullableInt(dr, "estMinutes"),
                        recordStatus = GetString(dr, "recordStatus") ?? "A",
                        usedInTemplates = GetInt(dr, "usedInTemplates")
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching checklist items.", ex);
            }
        }

        public HkChkSaveResult SaveItem(string sessionId, HkChkItemSaveRequest r)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_HK_CHK_ITEM_SAVE", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_ITEM_ID", r.itemId);
                cmd.Parameters.AddWithValue("@IP_ITEM_NM", r.itemNm ?? "");
                cmd.Parameters.AddWithValue("@IP_ITEM_CATEGORY", (object?)r.itemCategory ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IP_PHOTO_REQUIRED", YN(r.photoRequired));
                cmd.Parameters.AddWithValue("@IP_IS_MANDATORY", YN(r.isMandatory));
                cmd.Parameters.AddWithValue("@IP_INSTRUCTIONS", (object?)r.instructions ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IP_EST_MINUTES", (object?)r.estMinutes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IP_STATUS", r.recordStatus == "I" ? "I" : "A");

                return RunSave(cmd, conn);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while saving checklist item.", ex);
            }
        }

        // ==========================================
        // TEMPLATES
        // ==========================================

        public List<HkChkTemplate> GetTemplates(string sessionId)
        {
            var list = new List<HkChkTemplate>();
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_HK_CHK_TEMPLATE_LIST", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

                conn.Open();
                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new HkChkTemplate
                    {
                        templateId = GetInt(dr, "templateId"),
                        templateCd = GetString(dr, "templateCd") ?? "",
                        templateNm = GetString(dr, "templateNm") ?? "",
                        taskType = GetString(dr, "taskType") ?? "",
                        accmTypeId = GetNullableInt(dr, "accmTypeId"),
                        accmTypeName = GetString(dr, "accmTypeName"),
                        propertyId = GetNullableInt(dr, "propertyId"),
                        propertyName = GetString(dr, "propertyName"),
                        slaMinutes = GetInt(dr, "slaMinutes"),
                        slaWarnPct = GetInt(dr, "slaWarnPct"),
                        slaCriticalPct = GetInt(dr, "slaCriticalPct"),
                        inspectionRequired = GetString(dr, "inspectionRequired") ?? "Y",
                        remarks = GetString(dr, "remarks"),
                        recordStatus = GetString(dr, "recordStatus") ?? "A",
                        itemCount = GetInt(dr, "itemCount"),
                        photoCount = GetInt(dr, "photoCount"),
                        mandatoryCount = GetInt(dr, "mandatoryCount")
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching checklist templates.", ex);
            }
        }

        public List<HkChkTemplateItem> GetTemplateItems(string sessionId, int templateId)
        {
            var list = new List<HkChkTemplateItem>();
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_HK_CHK_TEMPLATE_ITEMS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_TEMPLATE_ID", templateId);

                conn.Open();
                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new HkChkTemplateItem
                    {
                        itemId = GetInt(dr, "itemId"),
                        itemCd = GetString(dr, "itemCd") ?? "",
                        itemNm = GetString(dr, "itemNm") ?? "",
                        itemCategory = GetString(dr, "itemCategory"),
                        instructions = GetString(dr, "instructions"),
                        sortOrder = GetInt(dr, "sortOrder"),
                        isMandatory = GetString(dr, "isMandatory") ?? "Y",
                        photoRequired = GetString(dr, "photoRequired") ?? "Y",
                        itemStatus = GetString(dr, "itemStatus") ?? "A"
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching template items.", ex);
            }
        }

        public HkChkSaveResult SaveTemplate(string sessionId, HkChkTemplateSaveRequest r)
        {
            try
            {
                var items = (r.items ?? new List<HkChkTemplateItemInput>())
                    .Select((x, i) => new
                    {
                        itemId = x.itemId,
                        sortOrder = i + 1,                 // screen order = checklist order
                        isMandatory = YN(x.isMandatory),
                        photoRequired = YN(x.photoRequired)
                    });

                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_HK_CHK_TEMPLATE_SAVE", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_TEMPLATE_ID", r.templateId);
                cmd.Parameters.AddWithValue("@IP_TEMPLATE_NM", r.templateNm ?? "");
                cmd.Parameters.AddWithValue("@IP_TASK_TYPE", r.taskType ?? "");
                cmd.Parameters.AddWithValue("@IP_ACCM_TYPE_ID", r.accmTypeId is > 0 ? (object)r.accmTypeId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@IP_PROPERTY_ID", r.propertyId is > 0 ? (object)r.propertyId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@IP_SLA_MINUTES", r.slaMinutes);
                cmd.Parameters.AddWithValue("@IP_SLA_WARN_PCT", r.slaWarnPct);
                cmd.Parameters.AddWithValue("@IP_SLA_CRITICAL_PCT", r.slaCriticalPct);
                cmd.Parameters.AddWithValue("@IP_INSPECTION_REQUIRED", YN(r.inspectionRequired));
                cmd.Parameters.AddWithValue("@IP_REMARKS", (object?)r.remarks ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IP_STATUS", r.recordStatus == "I" ? "I" : "A");
                cmd.Parameters.Add("@IP_ITEMS_JSON", SqlDbType.NVarChar, -1).Value = JsonSerializer.Serialize(items);

                return RunSave(cmd, conn);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while saving checklist template.", ex);
            }
        }

        // ==========================================
        // HELPERS
        // ==========================================

        private static HkChkSaveResult RunSave(SqlCommand cmd, SqlConnection conn)
        {
            SqlParameter id = new SqlParameter("@OUT_ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            SqlParameter status = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
            SqlParameter message = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(id);
            cmd.Parameters.Add(status);
            cmd.Parameters.Add(message);

            conn.Open();
            cmd.ExecuteNonQuery();

            return new HkChkSaveResult
            {
                id = id.Value == DBNull.Value ? 0 : Convert.ToInt32(id.Value),
                status = Convert.ToString(status.Value) ?? "",
                message = Convert.ToString(message.Value) ?? ""
            };
        }

        private static string YN(string? v) => string.Equals(v, "N", StringComparison.OrdinalIgnoreCase) ? "N" : "Y";

        private static string? GetString(SqlDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? null : Convert.ToString(dr[column]);
        }

        private static int GetInt(SqlDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? 0 : Convert.ToInt32(dr[column]);
        }

        private static int? GetNullableInt(SqlDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? null : Convert.ToInt32(dr[column]);
        }
    }
}
