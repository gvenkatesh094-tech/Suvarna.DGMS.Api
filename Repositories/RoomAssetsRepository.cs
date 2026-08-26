using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class RoomAssetsRepository
    {
        private readonly string _conn;

        public RoomAssetsRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        // ==========================================
        // GET ASSETS
        // ==========================================

        public List<RoomAssetRow> GetAssets(int sessionId)
        {
            List<RoomAssetRow> list = new();

            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_ROOM_ASSETS", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

            conn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new RoomAssetRow
                {
                    assetId = GetInt(dr, "ASSET_ID"),
                    assetNo = GetString(dr, "ASSET_NO") ?? "",
                    roomId = GetInt(dr, "ROOM_ID"),
                    roomNo = GetString(dr, "ROOM_NO") ?? "",
                    assetType = GetString(dr, "ASSET_TYPE") ?? "",
                    assetNm = GetString(dr, "ASSET_NM") ?? "",
                    brand = GetString(dr, "BRAND"),
                    modelNo = GetString(dr, "MODEL_NO"),
                    serialNo = GetString(dr, "SERIAL_NO"),
                    purchaseDt = GetNullableDateTime(dr, "PURCHASE_DT"),
                    purchaseCostAmt = GetNullableDecimal(dr, "PURCHASE_COST_AMT"),
                    warrantyExpiryDt = GetNullableDateTime(dr, "WARRANTY_EXPIRY_DT"),
                    lastMaintDt = GetNullableDateTime(dr, "LAST_MAINT_DT"),
                    nextPmDt = GetNullableDateTime(dr, "NEXT_PM_DT"),
                    defectCount = GetInt(dr, "DEFECT_COUNT"),
                    totalRepairCostAmt = GetDecimal(dr, "TOTAL_REPAIR_COST_AMT"),
                    assetStatus = GetString(dr, "ASSET_STATUS") ?? "",
                    remarks = GetString(dr, "REMARKS"),
                    isWarrantyExpired = GetString(dr, "IS_WARRANTY_EXPIRED") == "Y"
                });
            }

            return list;
        }

        // ==========================================
        // GET ROOMS (for the dropdown)
        // ==========================================

        public List<AssetRoomLookup> GetRooms(int sessionId)
        {
            List<AssetRoomLookup> list = new();

            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_ROOMS_FOR_ASSET", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

            conn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new AssetRoomLookup
                {
                    roomId = GetInt(dr, "ROOM_ID"),
                    roomNo = GetString(dr, "ROOM_NO") ?? "",
                    accmTypeNm = GetString(dr, "ACCM_TYPE_NM") ?? ""
                });
            }

            return list;
        }

        // ==========================================
        // SAVE (ADD / EDIT)
        // ==========================================

        public SaveAssetResult SaveAsset(SaveRoomAssetRequest request)
        {
            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_SAVE_ROOM_ASSET", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IP_SESSION_ID", request.sessionId);
            cmd.Parameters.AddWithValue("@IP_ASSET_ID", (object?)request.assetId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IP_ASSET_NO", request.assetNo);
            cmd.Parameters.AddWithValue("@IP_ROOM_ID", request.roomId);
            cmd.Parameters.AddWithValue("@IP_ASSET_TYPE", request.assetType);
            cmd.Parameters.AddWithValue("@IP_BRAND", string.IsNullOrWhiteSpace(request.brand) ? DBNull.Value : request.brand);
            cmd.Parameters.AddWithValue("@IP_MODEL_NO", string.IsNullOrWhiteSpace(request.modelNo) ? DBNull.Value : request.modelNo);
            cmd.Parameters.AddWithValue("@IP_SERIAL_NO", string.IsNullOrWhiteSpace(request.serialNo) ? DBNull.Value : request.serialNo);
            cmd.Parameters.AddWithValue("@IP_PURCHASE_DT", (object?)request.purchaseDt ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IP_PURCHASE_COST_AMT", (object?)request.purchaseCostAmt ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IP_WARRANTY_EXPIRY_DT", (object?)request.warrantyExpiryDt ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IP_ASSET_STATUS", string.IsNullOrWhiteSpace(request.assetStatus) ? "ACTIVE" : request.assetStatus);
            cmd.Parameters.AddWithValue("@IP_REMARKS", string.IsNullOrWhiteSpace(request.remarks) ? DBNull.Value : request.remarks);

            var outId = new SqlParameter("@OUT_ASSET_ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
            var outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outId);
            cmd.Parameters.Add(outStatus);
            cmd.Parameters.Add(outMessage);

            conn.Open();
            cmd.ExecuteNonQuery();

            return new SaveAssetResult
            {
                assetId = outId.Value == DBNull.Value ? 0 : Convert.ToInt32(outId.Value),
                status = Convert.ToString(outStatus.Value) ?? "",
                message = Convert.ToString(outMessage.Value) ?? ""
            };
        }

        // ==========================================
        // LOG REPAIR (not wired into UI yet)
        // ==========================================

        public AssetActionResult LogRepair(LogAssetRepairRequest request)
        {
            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_LOG_ASSET_REPAIR", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IP_SESSION_ID", request.sessionId);
            cmd.Parameters.AddWithValue("@IP_ASSET_ID", request.assetId);
            cmd.Parameters.AddWithValue("@IP_REPAIR_COST", request.repairCost);
            cmd.Parameters.AddWithValue("@IP_NEW_STATUS", request.newStatus);
            cmd.Parameters.AddWithValue("@IP_REMARKS", string.IsNullOrWhiteSpace(request.remarks) ? DBNull.Value : request.remarks);

            var outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
            var outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outStatus);
            cmd.Parameters.Add(outMessage);

            conn.Open();
            cmd.ExecuteNonQuery();

            return new AssetActionResult
            {
                status = Convert.ToString(outStatus.Value) ?? "",
                message = Convert.ToString(outMessage.Value) ?? ""
            };
        }

        // ==========================================
        // ASSETS DUE FOR PM (not wired into UI yet)
        // ==========================================

        public List<AssetDuePmRow> GetAssetsDueForPm(int sessionId)
        {
            List<AssetDuePmRow> list = new();

            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_ASSETS_DUE_FOR_PM", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

            conn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new AssetDuePmRow
                {
                    assetId = GetInt(dr, "ASSET_ID"),
                    assetNo = GetString(dr, "ASSET_NO") ?? "",
                    assetNm = GetString(dr, "ASSET_NM") ?? "",
                    roomNo = GetString(dr, "ROOM_NO") ?? "",
                    nextPmDt = GetNullableDateTime(dr, "NEXT_PM_DT")
                });
            }

            return list;
        }

        // ==========================================
        // COMMON HELPERS
        // ==========================================

        private static string? GetString(SqlDataReader dr, string column) =>
            dr[column] == DBNull.Value ? null : Convert.ToString(dr[column]);

        private static int GetInt(SqlDataReader dr, string column) =>
            dr[column] == DBNull.Value ? 0 : Convert.ToInt32(dr[column]);

        private static decimal GetDecimal(SqlDataReader dr, string column) =>
            dr[column] == DBNull.Value ? 0 : Convert.ToDecimal(dr[column]);

        private static decimal? GetNullableDecimal(SqlDataReader dr, string column) =>
            dr[column] == DBNull.Value ? null : Convert.ToDecimal(dr[column]);

        private static DateTime? GetNullableDateTime(SqlDataReader dr, string column) =>
            dr[column] == DBNull.Value ? null : Convert.ToDateTime(dr[column]);
    }
}
