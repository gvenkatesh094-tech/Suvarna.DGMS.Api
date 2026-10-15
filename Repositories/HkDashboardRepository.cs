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

        public List<HkTask> GetTasks(string sessionId, int? propertyId, string? priority, string? status, bool slaBreachOnly)
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
                        roomName = GetString(dr, "roomName") ?? "",
                        bookingId = GetNullableInt(dr, "bookingId"),
                        taskType = GetString(dr, "taskType") ?? "",
                        priority = GetString(dr, "priority") ?? "",
                        status = GetString(dr, "status") ?? "",
                        assignedUserId = GetNullableInt(dr, "assignedUserId"),
                        assignedStaffName = GetString(dr, "assignedStaffName"),
                        slaMinutes = GetInt(dr, "slaMinutes"),
                        triggeredDt = Convert.ToDateTime(dr["triggeredDt"]),
                        assignedDt = GetNullableDateTime(dr, "assignedDt"),
                        startedAt = GetNullableDateTime(dr, "startedAt"),
                        completedAt = GetNullableDateTime(dr, "completedAt"),
                        inspectedAt = GetNullableDateTime(dr, "inspectedAt"),
                        slaBreachFlag = GetString(dr, "slaBreachFlag") ?? "N",
                        inspectionRequired = GetString(dr, "inspectionRequired") ?? "Y",
                        failCount = GetInt(dr, "failCount"),
                        remarks = GetString(dr, "remarks"),
                        checklistTotal = GetInt(dr, "checklistTotal"),
                        checklistDone = GetInt(dr, "checklistDone"),
                        photoUrl = GetString(dr, "photoUrl"),

                        photoNotes = GetString(dr, "photoNotes")
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
        // NEW: TASK AUDIT TRAIL (points 2 & 6)
        // ==========================================

        public List<HkTaskHistoryEntry> GetTaskHistory(string sessionId, int taskId)
        {
            List<HkTaskHistoryEntry> list = new List<HkTaskHistoryEntry>();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_HK_TASK_HISTORY", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_HK_TASK_ID", taskId);

                conn.Open();
                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new HkTaskHistoryEntry
                    {
                        historyId = GetInt(dr, "historyId"),
                        actionType = GetString(dr, "actionType") ?? "",
                        actionDt = Convert.ToDateTime(dr["actionDt"]),
                        actionByName = GetString(dr, "actionByName"),
                        staffName = GetString(dr, "staffName"),
                        remarks = GetString(dr, "remarks")
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching task history.", ex);
            }
        }

        // ==========================================
        // NEW: REQUEST NEW TASK
        // ==========================================

        public HkOperationResult RequestNewTask(string sessionId)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_REQUEST_NEW_HK_TASK", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

                return RunWithOutputParams(cmd, conn);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while submitting task request.", ex);
            }
        }

        public List<HkTaskRequest> GetTaskRequests(string sessionId)
        {
            List<HkTaskRequest> list = new List<HkTaskRequest>();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_HK_TASK_REQUESTS", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

                conn.Open();
                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new HkTaskRequest
                    {
                        requestId = GetInt(dr, "requestId"),
                        staffUserId = GetInt(dr, "staffUserId"),
                        staffName = GetString(dr, "staffName"),
                        requestDt = Convert.ToDateTime(dr["requestDt"])
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching task requests.", ex);
            }
        }

        public HkOperationResult DismissTaskRequest(string sessionId, int requestId)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_DISMISS_HK_TASK_REQUEST", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_HK_TASK_REQUEST_ID", requestId);

                return RunWithOutputParams(cmd, conn);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while dismissing task request.", ex);
            }
        }

        // ==========================================
        // NEW: CREATE MANUAL TASK
        // ==========================================

        public HkOperationResult CreateManualTask(CreateManualHkTaskRequest request)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_CREATE_MANUAL_HK_TASK", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", request.sessionId);
                cmd.Parameters.AddWithValue("@IP_ROOM_ID", request.roomId);
                cmd.Parameters.AddWithValue("@IP_TASK_TYPE", request.taskType);
                cmd.Parameters.AddWithValue("@IP_PRIORITY", (object?)request.priority ?? DBNull.Value);

                // This proc has an extra OUTPUT parameter (the new task's
                // id) beyond the usual @OUT_STATUS/@OUT_MESSAGE pair that
                // RunWithOutputParams itself adds — must be supplied
                // explicitly or SQL Server will reject the call.
                var outTaskId = new SqlParameter("@OUT_HK_TASK_ID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outTaskId);

                return RunWithOutputParams(cmd, conn);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while creating task.", ex);
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
                        roomName = GetString(dr, "roomName") ?? "",
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
        // GET MY TASKS
        // ==========================================

        public List<HkTask> GetMyTasks(string sessionId)
        {
            List<HkTask> list = new List<HkTask>();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);

                using SqlCommand cmd =
                    new SqlCommand("DGMS.PR_DGMS_GET_MY_HK_TASKS", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

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
                        roomName = GetString(dr, "roomName") ?? "",
                        bookingId = GetNullableInt(dr, "bookingId"),
                        taskType = GetString(dr, "taskType") ?? "",
                        priority = GetString(dr, "priority") ?? "",
                        status = GetString(dr, "status") ?? "",
                        assignedUserId = GetNullableInt(dr, "assignedUserId"),
                        assignedStaffName = GetString(dr, "assignedStaffName"),
                        slaMinutes = GetInt(dr, "slaMinutes"),
                        triggeredDt = Convert.ToDateTime(dr["triggeredDt"]),
                        assignedDt = GetNullableDateTime(dr, "assignedDt"),
                        startedAt = GetNullableDateTime(dr, "startedAt"),
                        completedAt = GetNullableDateTime(dr, "completedAt"),
                        inspectedAt = GetNullableDateTime(dr, "inspectedAt"),
                        slaBreachFlag = GetString(dr, "slaBreachFlag") ?? "N",
                        inspectionRequired = GetString(dr, "inspectionRequired") ?? "Y",
                        failCount = GetInt(dr, "failCount"),
                        remarks = GetString(dr, "remarks"),
                        checklistTotal = GetInt(dr, "checklistTotal"),
                        checklistDone = GetInt(dr, "checklistDone"),
                        photoUrl = GetString(dr, "photoUrl"),

                        photoNotes = GetString(dr, "photoNotes")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching my housekeeping tasks.", ex);
            }
        }




        // ==========================================
        // UPLOAD PHOTO
        // ==========================================

        public HkOperationResult UploadPhoto(string sessionId, int taskId, string? notes, IFormFile photo)
        {
            try
            {
                // Image folder
                string uploadFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "uploads",
                    "housekeeping");

                // Folder create if not exists
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                // File name
                string extension = Suvarna.DGMS.Api.Helpers.UploadValidator.SafeExtension(photo);

                string fileName =
                    $"HK_{taskId}_{DateTime.Now:yyyyMMddHHmmss}{extension}";

                // Physical path
                string filePath =
                    Path.Combine(uploadFolder, fileName);

                // Save image
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    photo.CopyTo(stream);
                }

                // Database path
                string photoUrl =
                    "/uploads/housekeeping/" + fileName;

                using SqlConnection conn =
                    new SqlConnection(_conn);

                using SqlCommand cmd =
                    new SqlCommand(
                        "DGMS.PR_DGMS_UPLOAD_HK_PHOTO",
                        conn);

                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@IP_SESSION_ID",
                    sessionId);

                cmd.Parameters.AddWithValue(
                    "@IP_HK_TASK_ID",
                    taskId);

                cmd.Parameters.AddWithValue(
                    "@IP_PHOTO_URL",
                    photoUrl);

                cmd.Parameters.AddWithValue(
                    "@IP_NOTES",
                    (object?)notes ?? DBNull.Value);

                var result = RunWithOutputParams(cmd, conn);

                result.photoUrl = photoUrl;

                result.photoNotes = notes;
                return result;

            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Error while uploading photo.",
                    ex);
            }
        }
        // ==========================================
        // NEW: UPLOAD CHECKLIST ITEM PHOTO
        // Mirrors UploadPhoto() above but scoped to a single checklist
        // item (itemCd) instead of the whole task, and stores under its
        // own subfolder so item photos don't mix with task-level "Photo
        // Proof" uploads. Calls a NEW stored proc — see
        // PR_DGMS_UPLOAD_HK_CHECKLIST_ITEM_PHOTO.sql. That proc only
        // stamps PHOTO_URL on the checklist item; it deliberately does
        // NOT flip IS_COMPLETE, since the frontend already makes a
        // separate PR_DGMS_TOGGLE_HK_CHECKLIST_ITEM call right after a
        // successful upload to mark the item complete.
        // ==========================================

        public HkOperationResult UploadChecklistItemPhoto(string sessionId, int taskId, string itemCd, IFormFile photo)
        {
            try
            {
                string uploadFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "uploads",
                    "housekeeping",
                    "checklist");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string extension = Suvarna.DGMS.Api.Helpers.UploadValidator.SafeExtension(photo);

                string fileName =
                    $"HK_{taskId}_{System.Text.RegularExpressions.Regex.Replace(itemCd ?? "", "[^A-Za-z0-9_-]", "_")}_{DateTime.Now:yyyyMMddHHmmss}{extension}";   // itemCd comes from the client: strip "../" etc.

                string filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    photo.CopyTo(stream);
                }

                string photoUrl = "/uploads/housekeeping/checklist/" + fileName;

                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_UPLOAD_HK_CHECKLIST_ITEM_PHOTO", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_HK_TASK_ID", taskId);
                cmd.Parameters.AddWithValue("@IP_ITEM_CD", itemCd);
                cmd.Parameters.AddWithValue("@IP_PHOTO_URL", photoUrl);

                var result = RunWithOutputParams(cmd, conn);
                result.photoUrl = photoUrl;

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Error while uploading checklist item photo.",
                    ex);
            }
        }

        public HkQuickStats GetQuickStats(int sessionId, int locationId)
        {
            HkQuickStats stats = new HkQuickStats();

            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd =
                new SqlCommand("DGMS.PR_DGMS_GET_HK_QUICK_STATS", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
            cmd.Parameters.AddWithValue("@IP_LOCATION_ID", locationId);

            conn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                stats.avgTaskCompletionTime = GetString(dr, "avgTaskCompletionTime") ?? "0 min";
                stats.roomsCleanedToday = GetInt(dr, "roomsCleanedToday");
                stats.vipCleanings = GetInt(dr, "vipCleanings");
                stats.complaints = GetInt(dr, "complaints");
            }

            return stats;
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


        // ==========================================
        // GET ESCALATIONS
        // ==========================================

        public List<HkEscalation> GetEscalations(int sessionId)
        {
            List<HkEscalation> list = new List<HkEscalation>();
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_HK_ESCALATIONS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

                conn.Open();
                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new HkEscalation
                    {
                        taskId = GetInt(dr, "taskId"),
                        taskNo = GetString(dr, "taskNo") ?? "",
                        roomNo = GetString(dr, "roomNo") ?? "",
                        roomName = GetString(dr, "roomName") ?? "",
                        taskType = GetString(dr, "taskType") ?? "",
                        status = GetString(dr, "status") ?? "",
                        assignedUserId = GetNullableInt(dr, "assignedUserId"),
                        assignedStaffName = GetString(dr, "assignedStaffName"),
                        slaMinutes = GetInt(dr, "slaMinutes"),
                        triggeredDt = Convert.ToDateTime(dr["triggeredDt"]),
                        escalatedDt = GetNullableDateTime(dr, "escalatedDt")
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching HK escalations.", ex);
            }
        }

        // ==========================================
        // GET MY NOTIFICATIONS
        // ==========================================

        public List<HkNotification> GetMyNotifications(int sessionId)
        {
            List<HkNotification> list = new List<HkNotification>();
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_MY_HK_NOTIFICATIONS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

                conn.Open();
                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new HkNotification
                    {
                        notificationId = GetInt(dr, "notificationId"),
                        taskId = GetNullableInt(dr, "taskId"),
                        notificationType = GetString(dr, "notificationType") ?? "",
                        message = GetString(dr, "message") ?? "",
                        isRead = GetString(dr, "isRead") ?? "N",
                        createdDt = Convert.ToDateTime(dr["createdDt"])
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching notifications.", ex);
            }
        }

        // ==========================================
        // MARK NOTIFICATION READ (pass null notificationId to mark ALL read)
        // ==========================================

        public HkOperationResult MarkNotificationRead(int sessionId, int? notificationId)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_MARK_HK_NOTIFICATION_READ", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_NOTIFICATION_ID", (object?)notificationId ?? DBNull.Value);

                return RunWithOutputParams(cmd, conn);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while marking notification read.", ex);
            }
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
