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
        // GRID
        // ==========================================

        public List<MaintenanceRequestListItem> GetMaintRequests(
            int sessionId, string? priority, string? status, string? category,
            int? roomId, DateTime? fromDt, DateTime? toDt, int? closedDays)
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
                AddParameter(cmd, "@IP_CLOSED_DAYS", closedDays);

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
                        roomName = GetString(dr, "ROOM_NM"),
                        roomStatus = GetString(dr, "ROOM_STATUS_CD"),
                        bookingId = GetNullableInt(dr, "BOOKING_ID"),

                        category = GetString(dr, "CATEGORY") ?? "",
                        description = GetString(dr, "DESCRIPTION") ?? "",
                        priority = GetString(dr, "PRIORITY") ?? "",
                        maintStatus = GetString(dr, "MAINT_STATUS") ?? "",

                        raisedByType = GetString(dr, "RAISED_BY_TYPE") ?? "",
                        raisedByGuestName = GetString(dr, "RAISED_BY_GUEST_NM"),

                        assignedStaffId = GetNullableInt(dr, "ASSIGNED_STAFF_ID"),
                        assignedToName = GetString(dr, "ASSIGNED_TO_NAME"),
                        assignedToMobile = GetString(dr, "ASSIGNED_TO_MOBILE"),

                        roomBlockRequired = GetString(dr, "ROOM_BLOCK_REQUIRED") ?? "",
                        roomBlocked = GetString(dr, "ROOM_BLOCKED") ?? "",

                        raisedDt = GetUtc(dr, "RAISED_DT") ?? DateTime.MinValue,
                        acknowledgedDt = GetUtc(dr, "ACKNOWLEDGED_DT"),
                        workStartedDt = GetUtc(dr, "WORK_STARTED_DT"),
                        workCompletedDt = GetUtc(dr, "WORK_COMPLETED_DT"),
                        closedDt = GetUtc(dr, "CLOSED_DT"),
                        slaTargetDt = GetUtc(dr, "SLA_TARGET_DT"),

                        qcResult = GetString(dr, "QC_RESULT"),
                        qcRemarks = GetString(dr, "QC_REMARKS"),
                        resolutionRemarks = GetString(dr, "RESOLUTION_REMARKS"),
                        costAmount = GetDecimal(dr, "COST_AMT"),
                        reworkCount = GetInt(dr, "REWORK_COUNT"),

                        isOverdue = GetBool(dr, "IS_OVERDUE")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching maintenance requests: " + ex.Message, ex);
            }
        }

        // ==========================================
        // ROOM LOOKUP
        // ==========================================

        public List<MaintenanceRoomOption> GetRoomLookup(int sessionId)
        {
            List<MaintenanceRoomOption> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_ROOM_LOOKUP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId.ToString());   // proc takes VARCHAR

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
                throw new Exception("Error while fetching room lookup: " + ex.Message, ex);
            }
        }

        // ==========================================
        // RAISE
        // ==========================================

        public RaiseMaintRequestResult RaiseRequest(RaiseMaintRequestPayload p)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_RAISE_MAINT_REQ", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", p.sessionId.ToString());   // proc takes VARCHAR
                cmd.Parameters.AddWithValue("@IP_ROOM_ID", p.roomId);
                AddParameter(cmd, "@IP_BOOKING_ID", p.bookingId);
                cmd.Parameters.AddWithValue("@IP_COMPLAINT_CAT", p.complaintCategory);
                cmd.Parameters.AddWithValue("@IP_DESCRIPTION", p.description);
                AddParameter(cmd, "@IP_PRIORITY", string.IsNullOrWhiteSpace(p.priority) ? null : p.priority);
                cmd.Parameters.AddWithValue("@IP_RAISED_BY_TYPE", string.IsNullOrWhiteSpace(p.raisedByType) ? "STAFF" : p.raisedByType);
                cmd.Parameters.AddWithValue("@IP_BLOCK_ROOM", p.blockRoom == "Y" ? "Y" : "N");
                AddParameter(cmd, "@IP_RAISED_BY_NAME", string.IsNullOrWhiteSpace(p.raisedByName) ? null : p.raisedByName.Trim());

                var outId = new SqlParameter("@OUT_MAINT_ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var outNo = new SqlParameter("@OUT_MAINT_NO", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Output };
                var outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
                var outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outId);
                cmd.Parameters.Add(outNo);
                cmd.Parameters.Add(outStatus);
                cmd.Parameters.Add(outMessage);

                conn.Open();
                cmd.ExecuteNonQuery();

                return new RaiseMaintRequestResult
                {
                    maintReqId = outId.Value == DBNull.Value ? 0 : Convert.ToInt32(outId.Value),
                    maintNo = Convert.ToString(outNo.Value) ?? "",
                    status = Convert.ToString(outStatus.Value) ?? "",
                    message = Convert.ToString(outMessage.Value) ?? ""
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error while raising maintenance request: " + ex.Message, ex);
            }
        }

        // ==========================================
        // ASSIGN / REASSIGN (NEW)
        // ==========================================

        public MaintenanceActionResult AssignRequest(AssignMaintRequestPayload p)
        {
            return RunAction("DGMS.PR_DGMS_ASSIGN_MAINT_REQ", "assigning technician", cmd =>
            {
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", p.sessionId);
                cmd.Parameters.AddWithValue("@IP_MAINT_REQ_ID", p.maintReqId);
                cmd.Parameters.AddWithValue("@IP_STAFF_ID", p.staffId);
                AddParameter(cmd, "@IP_REMARKS", string.IsNullOrWhiteSpace(p.remarks) ? null : p.remarks.Trim());
            });
        }

        // ==========================================
        // STATUS ACTIONS
        // ==========================================

        public MaintenanceActionResult AcknowledgeRequest(int sessionId, int maintReqId) =>
            RunAction("DGMS.PR_DGMS_ACKNOWLEDGE_MAINT_REQ", "acknowledging request", cmd =>
            {
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_MAINT_REQ_ID", maintReqId);
            });

        public MaintenanceActionResult StartWork(StartMaintWorkPayload p) =>
            RunAction("DGMS.PR_DGMS_START_MAINT_WORK", "starting work", cmd =>
            {
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", p.sessionId);
                cmd.Parameters.AddWithValue("@IP_MAINT_REQ_ID", p.maintReqId);
            });

        public MaintenanceActionResult ResolveRequest(ResolveMaintRequestPayload p) =>
            RunAction("DGMS.PR_DGMS_RESOLVE_MAINT_REQ", "resolving request", cmd =>
            {
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", p.sessionId);
                cmd.Parameters.AddWithValue("@IP_MAINT_REQ_ID", p.maintReqId);
                cmd.Parameters.AddWithValue("@IP_RESOLUTION_REMARKS", p.resolutionRemarks);
                AddParameter(cmd, "@IP_MATERIAL_USED", string.IsNullOrWhiteSpace(p.materialUsed) ? null : p.materialUsed.Trim());
                AddDecimal(cmd, "@IP_MATERIAL_COST", p.materialCost, 18, 2);
                AddDecimal(cmd, "@IP_LABOUR_HRS", p.labourHrs, 5, 2);
                AddDecimal(cmd, "@IP_LABOUR_COST", p.labourCost, 18, 2);
            });

        public MaintenanceActionResult CloseRequest(int sessionId, int maintReqId) =>
            RunAction("DGMS.PR_DGMS_CLOSE_MAINT_REQ", "closing request", cmd =>
            {
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_MAINT_REQ_ID", maintReqId);
            });

        public MaintenanceActionResult SubmitQc(SubmitMaintQcPayload p) =>
            RunAction("DGMS.PR_DGMS_SUBMIT_MAINT_QC", "submitting QC", cmd =>
            {
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", p.sessionId);
                cmd.Parameters.AddWithValue("@IP_MAINT_REQ_ID", p.maintReqId);
                cmd.Parameters.AddWithValue("@IP_QC_RESULT", p.qcResult);
                AddParameter(cmd, "@IP_QC_REMARKS", string.IsNullOrWhiteSpace(p.qcRemarks) ? null : p.qcRemarks.Trim());
            });

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
                        createDt = GetUtc(dr, "CREATE_DT") ?? DateTime.MinValue
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching maintenance work log: " + ex.Message, ex);
            }
        }

        // ==========================================
        // ACTIVITY LOG (NEW)
        // ==========================================

        public List<MaintenanceActivityItem> GetActivityLog(int sessionId, int maintReqId)
        {
            List<MaintenanceActivityItem> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_MAINT_REQ_LOG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_MAINT_REQ_ID", maintReqId);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new MaintenanceActivityItem
                    {
                        logId = Convert.ToInt64(dr["LOG_ID"]),
                        actionCd = GetString(dr, "ACTION_CD") ?? "",
                        fromStatus = GetString(dr, "FROM_STATUS"),
                        toStatus = GetString(dr, "TO_STATUS"),
                        staffName = GetString(dr, "STAFF_NM"),
                        remarks = GetString(dr, "REMARKS"),
                        actionBy = GetInt(dr, "ACTION_BY"),
                        actionDt = GetUtc(dr, "ACTION_DT") ?? DateTime.MinValue
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching activity log: " + ex.Message, ex);
            }
        }

        // ==========================================
        // TECHNICIANS (NEW)
        // ==========================================

        public List<MaintenanceStaffItem> GetStaff(int sessionId, bool includeInactive)
        {
            List<MaintenanceStaffItem> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_MAINT_STAFF", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_INCLUDE_INACTIVE", includeInactive ? "Y" : "N");

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new MaintenanceStaffItem
                    {
                        staffId = GetInt(dr, "STAFF_ID"),
                        staffName = GetString(dr, "STAFF_NM") ?? "",
                        mobileNo = GetString(dr, "MOBILE_NO"),
                        skillCats = GetString(dr, "SKILL_CATS"),
                        isActive = (GetString(dr, "RECORD_STATUS") ?? "A") == "A",
                        openJobs = GetInt(dr, "OPEN_JOBS")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching technicians: " + ex.Message, ex);
            }
        }

        public SaveMaintStaffResult SaveStaff(SaveMaintStaffPayload p)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_SAVE_MAINT_STAFF", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", p.sessionId);
                AddParameter(cmd, "@IP_STAFF_ID", p.staffId);
                cmd.Parameters.AddWithValue("@IP_STAFF_NM", p.staffName.Trim());
                AddParameter(cmd, "@IP_MOBILE_NO", string.IsNullOrWhiteSpace(p.mobileNo) ? null : p.mobileNo.Trim());
                AddParameter(cmd, "@IP_SKILL_CATS", string.IsNullOrWhiteSpace(p.skillCats) ? null : p.skillCats.Trim());
                cmd.Parameters.AddWithValue("@IP_ACTIVE", p.isActive ? "Y" : "N");

                var outId = new SqlParameter("@OUT_STAFF_ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
                var outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outId);
                cmd.Parameters.Add(outStatus);
                cmd.Parameters.Add(outMessage);

                conn.Open();
                cmd.ExecuteNonQuery();

                return new SaveMaintStaffResult
                {
                    staffId = outId.Value == DBNull.Value ? 0 : Convert.ToInt32(outId.Value),
                    status = Convert.ToString(outStatus.Value) ?? "",
                    message = Convert.ToString(outMessage.Value) ?? ""
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error while saving technician: " + ex.Message, ex);
            }
        }

        // ==========================================
        // HELPERS
        // ==========================================

        private MaintenanceActionResult RunAction(string proc, string what, Action<SqlCommand> addParams)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand(proc, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                addParams(cmd);

                var status = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
                var message = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(status);
                cmd.Parameters.Add(message);

                conn.Open();
                cmd.ExecuteNonQuery();

                return new MaintenanceActionResult
                {
                    status = Convert.ToString(status.Value) ?? "",
                    message = Convert.ToString(message.Value) ?? ""
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while {what}: " + ex.Message, ex);
            }
        }

        private static void AddParameter(SqlCommand cmd, string name, object? value) =>
            cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);

        private static void AddDecimal(SqlCommand cmd, string name, decimal? value, byte precision, byte scale) =>
            cmd.Parameters.Add(new SqlParameter(name, SqlDbType.Decimal)
            {
                Precision = precision,
                Scale = scale,
                Value = value.HasValue ? value.Value : DBNull.Value
            });

        private static string? GetString(SqlDataReader dr, string c) => dr[c] == DBNull.Value ? null : Convert.ToString(dr[c]);
        private static int GetInt(SqlDataReader dr, string c) => dr[c] == DBNull.Value ? 0 : Convert.ToInt32(dr[c]);
        private static int? GetNullableInt(SqlDataReader dr, string c) => dr[c] == DBNull.Value ? null : Convert.ToInt32(dr[c]);
        private static decimal GetDecimal(SqlDataReader dr, string c) => dr[c] == DBNull.Value ? 0 : Convert.ToDecimal(dr[c]);
        private static decimal? GetNullableDecimal(SqlDataReader dr, string c) => dr[c] == DBNull.Value ? null : Convert.ToDecimal(dr[c]);
        private static bool GetBool(SqlDataReader dr, string c) => dr[c] != DBNull.Value && Convert.ToBoolean(dr[c]);

        /// <summary>
        /// Every maintenance timestamp is written with GETUTCDATE(). Marking it
        /// as UTC makes the JSON carry a trailing "Z", so the browser shows the
        /// correct local (IST) time instead of a time 5h30m behind.
        /// </summary>
        private static DateTime? GetUtc(SqlDataReader dr, string c) =>
            dr[c] == DBNull.Value ? null : DateTime.SpecifyKind(Convert.ToDateTime(dr[c]), DateTimeKind.Utc);
    }
}
