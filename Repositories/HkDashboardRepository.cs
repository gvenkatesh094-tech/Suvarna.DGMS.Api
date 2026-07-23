using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class HkDashboardRepository
    {
        private readonly string _conn;

        public HkDashboardRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        // ==========================================
        // GET TASKS
        // ==========================================

        public List<HkTask> GetTasks(
            string sessionId,
            int? propertyId,
            string? priority,
            string? status,
            bool slaBreachOnly)
        {
            List<HkTask> list = new List<HkTask>();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_HK_TASKS", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_PROPERTY_ID", (object?)propertyId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IP_PRIORITY", (object?)priority ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IP_STATUS", (object?)status ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IP_SLA_BREACH_ONLY", slaBreachOnly ? "Y" : "N");

                conn.Open();
                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new HkTask
                    {
                        taskId = GetInt(dr, "taskId"),
                        taskNo = GetString(dr, "taskNo") ?? "",
                        roomId = GetInt(dr, "roomId"),
                        roomNo = GetString(dr, "roomNo") ?? "",
                        bookingId = GetNullableInt(dr, "bookingId"),
                        taskType = GetString(dr, "taskType") ?? "",
                        priority = GetString(dr, "priority") ?? "",
                        status = GetString(dr, "status") ?? "",
                        assignedUserId = GetNullableInt(dr, "assignedUserId"),
                        assignedStaffName = GetString(dr, "assignedStaffName"),
                        slaMinutes = GetInt(dr, "slaMinutes"),
                        triggeredDt = Convert.ToDateTime(dr["triggeredDt"]),
                        startedAt = GetNullableDateTime(dr, "startedAt"),
                        completedAt = GetNullableDateTime(dr, "completedAt"),
                        inspectedAt = GetNullableDateTime(dr, "inspectedAt"),
                        slaBreachFlag = GetString(dr, "slaBreachFlag") ?? "N",
                        inspectionRequired = GetString(dr, "inspectionRequired") ?? "Y",
                        failCount = GetInt(dr, "failCount"),
                        remarks = GetString(dr, "remarks"),
                        checklistTotal = GetInt(dr, "checklistTotal"),
                        checklistDone = GetInt(dr, "checklistDone")
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching housekeeping tasks.", ex);
            }
        }

        // ==========================================
        // GET CHECKLIST FOR A TASK
        // ==========================================

        public List<HkChecklistItem> GetTaskChecklist(string sessionId, int taskId)
        {
            List<HkChecklistItem> list = new List<HkChecklistItem>();
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_HK_TASK_CHECKLIST", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_HK_TASK_ID", taskId);

                conn.Open();
                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new HkChecklistItem
                    {
                        itemId = GetInt(dr, "itemId"),
                        itemSeq = GetInt(dr, "itemSeq"),
                        itemCd = GetString(dr, "itemCd") ?? "",
                        itemNm = GetString(dr, "itemNm") ?? "",
                        isComplete = GetString(dr, "isComplete") ?? "N",
                        completeDt = GetNullableDateTime(dr, "completeDt"),
                        photoUrl = GetString(dr, "photoUrl")
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching task checklist.", ex);
            }
        }

        // ==========================================
        // GET ROOM GRID
        // ==========================================

        public List<HkRoom> GetRoomGrid(string sessionId, int? propertyId, short? floorSeq)
        {
            List<HkRoom> list = new List<HkRoom>();
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_HK_ROOM_GRID", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_PROPERTY_ID", (object?)propertyId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IP_FLOOR_SEQ", (object?)floorSeq ?? DBNull.Value);

                conn.Open();
                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new HkRoom
                    {
                        roomId = GetInt(dr, "roomId"),
                        roomNo = GetString(dr, "roomNo") ?? "",
                        roomStatus = GetString(dr, "roomStatus") ?? "",
                        floorSeq = GetInt(dr, "floorSeq"),
                        floorName = GetString(dr, "floorName") ?? "",
                        accmTypeName = GetString(dr, "accmTypeName") ?? "",
                        accmTypeCode = GetString(dr, "accmTypeCode") ?? ""
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching room grid.", ex);
            }
        }

        // ==========================================
        // GET STAFF ON SHIFT
        // ==========================================

        public List<HkStaff> GetStaffOnShift(string sessionId, int? propertyId)
        {
            List<HkStaff> list = new List<HkStaff>();
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_HK_STAFF_SHIFT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_PROPERTY_ID", (object?)propertyId ?? DBNull.Value);

                conn.Open();
                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new HkStaff
                    {
                        userId = GetInt(dr, "userId"),
                        staffName = GetString(dr, "staffName") ?? "",
                        initials = GetString(dr, "initials") ?? "",
                        status = GetString(dr, "status") ?? "",
                        tasksAssigned = GetInt(dr, "tasksAssigned")
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching staff on shift.", ex);
            }
        }

        // ==========================================
        // ASSIGN TASK
        // ==========================================

        public HkOperationResult AssignTask(AssignHkTaskRequest request)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_ASSIGN_HK_TASK", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", request.sessionId);
                cmd.Parameters.AddWithValue("@IP_HK_TASK_ID", request.taskId);
                cmd.Parameters.AddWithValue("@IP_STAFF_USER_ID", request.staffUserId);
                cmd.Parameters.AddWithValue("@IP_PRIORITY", (object?)request.priority ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IP_NOTES", (object?)request.notes ?? DBNull.Value);

                return RunWithOutputParams(cmd, conn);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while assigning housekeeping task.", ex);
            }
        }

        // ==========================================
        // TOGGLE CHECKLIST ITEM
        // ==========================================

        public HkOperationResult ToggleChecklistItem(ToggleHkChecklistItemRequest request)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_TOGGLE_HK_CHECKLIST_ITEM", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", request.sessionId);
                cmd.Parameters.AddWithValue("@IP_HK_TASK_ID", request.taskId);
                cmd.Parameters.AddWithValue("@IP_ITEM_CD", request.itemCd);
                cmd.Parameters.AddWithValue("@IP_IS_COMPLETE", request.isComplete ? "Y" : "N");

                return RunWithOutputParams(cmd, conn);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while updating checklist item.", ex);
            }
        }

        // ==========================================
        // COMPLETE TASK
        // ==========================================

        public HkOperationResult CompleteTask(CompleteHkTaskRequest request)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_COMPLETE_HK_TASK", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", request.sessionId);
                cmd.Parameters.AddWithValue("@IP_HK_TASK_ID", request.taskId);

                return RunWithOutputParams(cmd, conn);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while completing housekeeping task.", ex);
            }
        }

        // ==========================================
        // INSPECT TASK
        // ==========================================

        public HkOperationResult InspectTask(InspectHkTaskRequest request)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_INSPECT_HK_TASK", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", request.sessionId);
                cmd.Parameters.AddWithValue("@IP_HK_TASK_ID", request.taskId);
                cmd.Parameters.AddWithValue("@IP_RESULT", request.result);
                cmd.Parameters.AddWithValue("@IP_FAIL_REMARKS", (object?)request.failRemarks ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IP_OVERALL_SCORE", (object?)request.overallScore ?? DBNull.Value);

                return RunWithOutputParams(cmd, conn);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while recording HK inspection.", ex);
            }
        }

        // ==========================================
        // COMMON HELPERS
        // ==========================================

        private static HkOperationResult RunWithOutputParams(SqlCommand cmd, SqlConnection conn)
        {
            SqlParameter status = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(status);

            SqlParameter message = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(message);

            conn.Open();
            cmd.ExecuteNonQuery();

            return new HkOperationResult
            {
                status = Convert.ToString(status.Value) ?? "",
                message = Convert.ToString(message.Value) ?? ""
            };
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

        private static DateTime? GetNullableDateTime(SqlDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? null : Convert.ToDateTime(dr[column]);
        }
    }
}
