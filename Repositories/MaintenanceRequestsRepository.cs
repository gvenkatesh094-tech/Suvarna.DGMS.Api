using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class MaintenanceRequestsRepository
    {
        private readonly string _conn;

        public MaintenanceRequestsRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        // ==========================================
        // GET MAINTENANCE REQUESTS (grid)
        // ==========================================

        public List<MaintenanceRequestListItem> GetMaintRequests(
            int sessionId,
            string? priority,
            string? status,
            string? category,
            int? roomId,
            DateTime? fromDt,
            DateTime? toDt)
        {
            List<MaintenanceRequestListItem> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_MAINT_REQUESTS", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                AddParameter(cmd, "@IP_PRIORITY", priority);
                AddParameter(cmd, "@IP_STATUS", status);
                AddParameter(cmd, "@IP_CATEGORY", category);
                AddParameter(cmd, "@IP_ROOM_ID", roomId);
                AddParameter(cmd, "@IP_FROM_DT", fromDt?.Date);
                AddParameter(cmd, "@IP_TO_DT", toDt?.Date);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new MaintenanceRequestListItem
                    {
                        maintReqId = GetInt(dr, "MAINT_REQ_ID"),
                        maintNo = GetString(dr, "MAINT_NO") ?? "",
                        roomId = GetInt(dr, "ROOM_ID"),
                        roomNo = GetString(dr, "ROOM_NO") ?? "",
                        bookingId = GetNullableInt(dr, "BOOKING_ID"),

                        category = GetString(dr, "CATEGORY") ?? "",
                        description = GetString(dr, "DESCRIPTION") ?? "",
                        priority = GetString(dr, "PRIORITY") ?? "",
                        maintStatus = GetString(dr, "MAINT_STATUS") ?? "",

                        raisedByType = GetString(dr, "RAISED_BY_TYPE") ?? "",
                        raisedByGuestName = GetString(dr, "RAISED_BY_GUEST_NM"),
                        assignedToName = GetString(dr, "ASSIGNED_TO_NAME"),

                        roomBlockRequired = GetString(dr, "ROOM_BLOCK_REQUIRED") ?? "",
                        roomBlocked = GetString(dr, "ROOM_BLOCKED") ?? "",

                        raisedDt = GetNullableDateTime(dr, "RAISED_DT") ?? DateTime.MinValue,
                        acknowledgedDt = GetNullableDateTime(dr, "ACKNOWLEDGED_DT"),
                        workStartedDt = GetNullableDateTime(dr, "WORK_STARTED_DT"),
                        workCompletedDt = GetNullableDateTime(dr, "WORK_COMPLETED_DT"),
                        closedDt = GetNullableDateTime(dr, "CLOSED_DT"),
                        slaTargetDt = GetNullableDateTime(dr, "SLA_TARGET_DT"),

                        qcResult = GetString(dr, "QC_RESULT"),
                        qcRemarks = GetString(dr, "QC_REMARKS"),
                        resolutionRemarks = GetString(dr, "RESOLUTION_REMARKS"),
                        costAmount = GetDecimal(dr, "COST_AMT"),

                        isOverdue = GetBool(dr, "IS_OVERDUE")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching maintenance requests.", ex);
            }
        }

        // ==========================================
        // GET ROOM LOOKUP (reuses existing PR_DGMS_GET_ROOM_LOOKUP)
        // ==========================================

        public List<MaintenanceRoomOption> GetRoomLookup(int sessionId)
        {
            List<MaintenanceRoomOption> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_ROOM_LOOKUP", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // This existing proc takes the session id as VARCHAR(100).
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId.ToString());

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new MaintenanceRoomOption
                    {
                        roomId = GetInt(dr, "ROOM_ID"),
                        roomNo = GetString(dr, "ROOM_NO") ?? "",
                        roomName = GetString(dr, "ROOM_NM"),
                        roomStatus = GetString(dr, "ROOM_STATUS_CD"),
                        accmTypeName = GetString(dr, "ACCM_TYPE_NM")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching room lookup.", ex);
            }
        }

        // ==========================================
        // RAISE REQUEST (reuses existing PR_DGMS_RAISE_MAINT_REQ)
        // ==========================================

        public RaiseMaintRequestResult RaiseRequest(RaiseMaintRequestPayload payload)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_RAISE_MAINT_REQ", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // This existing proc takes the session id as VARCHAR(100).
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", payload.sessionId.ToString());
                cmd.Parameters.AddWithValue("@IP_ROOM_ID", payload.roomId);
                AddParameter(cmd, "@IP_BOOKING_ID", payload.bookingId);
                cmd.Parameters.AddWithValue("@IP_COMPLAINT_CAT", payload.complaintCategory);
                cmd.Parameters.AddWithValue("@IP_DESCRIPTION", payload.description);
                AddParameter(cmd, "@IP_PRIORITY", string.IsNullOrWhiteSpace(payload.priority) ? null : payload.priority);
                cmd.Parameters.AddWithValue("@IP_RAISED_BY_TYPE", string.IsNullOrWhiteSpace(payload.raisedByType) ? "STAFF" : payload.raisedByType);
                cmd.Parameters.AddWithValue("@IP_BLOCK_ROOM", string.IsNullOrWhiteSpace(payload.blockRoom) ? "N" : payload.blockRoom);

                SqlParameter outId = new SqlParameter("@OUT_MAINT_ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outId);

                SqlParameter outNo = new SqlParameter("@OUT_MAINT_NO", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outNo);

                SqlParameter status = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(status);

                SqlParameter message = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(message);

                conn.Open();
                cmd.ExecuteNonQuery();

                return new RaiseMaintRequestResult
                {
                    maintReqId = outId.Value == DBNull.Value ? 0 : Convert.ToInt32(outId.Value),
                    maintNo = Convert.ToString(outNo.Value) ?? "",
                    status = Convert.ToString(status.Value) ?? "",
                    message = Convert.ToString(message.Value) ?? ""
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error while raising maintenance request.", ex);
            }
        }

        // ==========================================
        // ACKNOWLEDGE / START / RESOLVE / CLOSE / QC
        // ==========================================

        public MaintenanceActionResult AcknowledgeRequest(int sessionId, int maintReqId)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_ACKNOWLEDGE_MAINT_REQ", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_MAINT_REQ_ID", maintReqId);

                return ExecuteActionProc(conn, cmd);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while acknowledging maintenance request.", ex);
            }
        }

        public MaintenanceActionResult StartWork(StartMaintWorkPayload payload)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_START_MAINT_WORK", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", payload.sessionId);
                cmd.Parameters.AddWithValue("@IP_MAINT_REQ_ID", payload.maintReqId);
                AddParameter(cmd, "@IP_ASSIGNED_TO_USER_ID", payload.assignedToUserId);
                AddParameter(cmd, "@IP_ASSIGNED_TO_NAME", payload.assignedToName);

                return ExecuteActionProc(conn, cmd);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while starting maintenance work.", ex);
            }
        }

        public MaintenanceActionResult ResolveRequest(ResolveMaintRequestPayload payload)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_RESOLVE_MAINT_REQ", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", payload.sessionId);
                cmd.Parameters.AddWithValue("@IP_MAINT_REQ_ID", payload.maintReqId);
                cmd.Parameters.AddWithValue("@IP_RESOLUTION_REMARKS", payload.resolutionRemarks);
                AddParameter(cmd, "@IP_MATERIAL_USED", payload.materialUsed);
                AddParameter(cmd, "@IP_MATERIAL_COST", payload.materialCost);
                AddParameter(cmd, "@IP_LABOUR_HRS", payload.labourHrs);
                AddParameter(cmd, "@IP_LABOUR_COST", payload.labourCost);

                return ExecuteActionProc(conn, cmd);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while resolving maintenance request.", ex);
            }
        }

        public MaintenanceActionResult CloseRequest(int sessionId, int maintReqId)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_CLOSE_MAINT_REQ", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_MAINT_REQ_ID", maintReqId);

                return ExecuteActionProc(conn, cmd);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while closing maintenance request.", ex);
            }
        }

        public MaintenanceActionResult SubmitQc(SubmitMaintQcPayload payload)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_SUBMIT_MAINT_QC", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", payload.sessionId);
                cmd.Parameters.AddWithValue("@IP_MAINT_REQ_ID", payload.maintReqId);
                cmd.Parameters.AddWithValue("@IP_QC_RESULT", payload.qcResult);
                AddParameter(cmd, "@IP_QC_REMARKS", payload.qcRemarks);

                return ExecuteActionProc(conn, cmd);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while submitting QC.", ex);
            }
        }

        // ==========================================
        // WORK LOG
        // ==========================================

        public List<MaintenanceWorkLogItem> GetWorkLog(int sessionId, int maintReqId)
        {
            List<MaintenanceWorkLogItem> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_MAINT_REQ_WORKLOG", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_MAINT_REQ_ID", maintReqId);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new MaintenanceWorkLogItem
                    {
                        detId = GetInt(dr, "DET_ID"),
                        lineNo = GetInt(dr, "LINE_NO"),
                        workDesc = GetString(dr, "WORK_DESC") ?? "",
                        materialUsed = GetString(dr, "MATERIAL_USED"),
                        materialCostAmount = GetDecimal(dr, "MATERIAL_COST_AMT"),
                        labourHrs = GetNullableDecimal(dr, "LABOUR_HRS"),
                        labourCostAmount = GetDecimal(dr, "LABOUR_COST_AMT"),
                        lineTotalAmount = GetDecimal(dr, "LINE_TOTAL_AMT"),
                        photoUrl = GetString(dr, "PHOTO_URL"),
                        remarks = GetString(dr, "REMARKS"),
                        createDt = GetNullableDateTime(dr, "CREATE_DT") ?? DateTime.MinValue
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching maintenance work log.", ex);
            }
        }

        // ==========================================
        // COMMON HELPERS
        // ==========================================

        private static MaintenanceActionResult ExecuteActionProc(SqlConnection conn, SqlCommand cmd)
        {
            SqlParameter status = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1)
            { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(status);

            SqlParameter message = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500)
            { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(message);

            conn.Open();
            cmd.ExecuteNonQuery();

            return new MaintenanceActionResult
            {
                status = Convert.ToString(status.Value) ?? "",
                message = Convert.ToString(message.Value) ?? ""
            };
        }

        private static void AddParameter(SqlCommand cmd, string name, object? value)
        {
            cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
        }

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

        private static decimal GetDecimal(SqlDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? 0 : Convert.ToDecimal(dr[column]);
        }

        private static decimal? GetNullableDecimal(SqlDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? null : Convert.ToDecimal(dr[column]);
        }

        private static bool GetBool(SqlDataReader dr, string column)
        {
            return dr[column] != DBNull.Value && Convert.ToBoolean(dr[column]);
        }

        private static DateTime? GetNullableDateTime(SqlDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? null : Convert.ToDateTime(dr[column]);
        }
    }
}
