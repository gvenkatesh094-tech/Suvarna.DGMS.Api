using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Helpers;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    /// <summary>Security module data access. Identity (group/org/location/user) is never taken from the client:
    /// every SP receives only the session id and resolves the rest itself.</summary>
    public class SecurityRepository
    {
        private readonly SessionContext _session;
        private readonly string _conn;

        public SecurityRepository(IConfiguration configuration, SessionContext session)
        {
            _session = session;
            _conn = configuration.GetConnectionString("SqlServer") ?? throw new InvalidOperationException("ConnectionStrings:SqlServer missing");
        }

        // ---------- helpers ----------
        private SqlCommand Cmd(SqlConnection c, string sp)
        {
            var cmd = new SqlCommand("DGMS." + sp, c) { CommandType = CommandType.StoredProcedure, CommandTimeout = 30 };
            cmd.Parameters.Add("@IP_SESSION_ID", SqlDbType.VarChar, 100).Value = _session.Require();
            return cmd;
        }
        private static object V(object? o) => o ?? DBNull.Value;
        private static object S(string? s) => string.IsNullOrWhiteSpace(s) ? DBNull.Value : s.Trim();
        // Query-string dates ("...Z") are bound as Local by MVC; the DB column is UTC, so always normalise before sending.
        private static DateTime Utc(DateTime d) => d.Kind == DateTimeKind.Local ? d.ToUniversalTime() : DateTime.SpecifyKind(d, DateTimeKind.Utc);
        private static object UtcN(DateTime? d) => d.HasValue ? Utc(d.Value) : DBNull.Value;
        private static string YN(bool b) => b ? "Y" : "N";
        private static string? Str(SqlDataReader r, string c) { int i = r.GetOrdinal(c); return r.IsDBNull(i) ? null : Convert.ToString(r.GetValue(i)); }
        private static int Int(SqlDataReader r, string c) { int i = r.GetOrdinal(c); return r.IsDBNull(i) ? 0 : Convert.ToInt32(r.GetValue(i)); }
        private static int? NInt(SqlDataReader r, string c) { int i = r.GetOrdinal(c); return r.IsDBNull(i) ? null : Convert.ToInt32(r.GetValue(i)); }
        private static long Long(SqlDataReader r, string c) { int i = r.GetOrdinal(c); return r.IsDBNull(i) ? 0 : Convert.ToInt64(r.GetValue(i)); }
        private static bool Bool(SqlDataReader r, string c) { int i = r.GetOrdinal(c); if (r.IsDBNull(i)) return false; var v = r.GetValue(i); return v is bool b ? b : (v is string s ? s == "Y" || s == "1" : Convert.ToInt32(v) != 0); }
        private static DateTime? Dt(SqlDataReader r, string c) { int i = r.GetOrdinal(c); return r.IsDBNull(i) ? null : DateTime.SpecifyKind(r.GetDateTime(i), DateTimeKind.Utc); }
        private static SqlParameter Out(SqlCommand cmd, string n, SqlDbType t, int size = 0)
        { var p = size > 0 ? cmd.Parameters.Add(n, t, size) : cmd.Parameters.Add(n, t); p.Direction = ParameterDirection.Output; return p; }

        // ---------- lookups & dashboard ----------
        public List<SecLookupItem> GetLookups()
        {
            var list = new List<SecLookupItem>();
            using var c = new SqlConnection(_conn); using var cmd = Cmd(c, "PR_DGMS_SEC_GET_LOOKUPS"); c.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(new SecLookupItem { category = Str(r, "CATEGORY") ?? "", code = Str(r, "CODE") ?? "", name = Str(r, "NAME") ?? "", extraInt = NInt(r, "EXTRA_INT"), sortOrder = Int(r, "SORT_ORDER") });
            return list;
        }

        public List<SecHost> GetHosts(string? search)
        {
            var list = new List<SecHost>();
            using var c = new SqlConnection(_conn); using var cmd = Cmd(c, "PR_DGMS_SEC_GET_HOSTS");
            cmd.Parameters.AddWithValue("@IP_SEARCH", S(search)); c.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(new SecHost { bookingId = Int(r, "BOOKING_ID"), bookingNo = Str(r, "BOOKING_NO"), propertyId = Int(r, "PROPERTY_ID"), roomId = Int(r, "ROOM_ID"), roomNo = Str(r, "ROOM_NO"), guestName = Str(r, "GUEST_NM"), mobileNo = Str(r, "MOBILE_NO") });
            return list;
        }

        public SecKpis GetKpis(DateTime fromUtc, DateTime toUtc)
        {
            using var c = new SqlConnection(_conn); using var cmd = Cmd(c, "PR_DGMS_SEC_GET_KPIS");
            cmd.Parameters.AddWithValue("@IP_FROM_UTC", Utc(fromUtc)); cmd.Parameters.AddWithValue("@IP_TO_UTC", Utc(toUtc)); c.Open();
            using var r = cmd.ExecuteReader(); var k = new SecKpis();
            if (r.Read()) k = new SecKpis { activeOccupants = Int(r, "ACTIVE_OCCUPANTS"), activePasses = Int(r, "ACTIVE_PASSES"), expiredPasses = Int(r, "EXPIRED_PASSES"), insideNow = Int(r, "INSIDE_NOW"), blacklistCount = Int(r, "BLACKLIST_COUNT"), entriesToday = Int(r, "ENTRIES_TODAY"), exitsToday = Int(r, "EXITS_TODAY"), deniedToday = Int(r, "DENIED_TODAY"), openIncidents = Int(r, "OPEN_INCIDENTS") };
            return k;
        }

        // ---------- visitor pass ----------
        public SecBlacklistCheckResult CheckBlacklist(SecBlacklistCheckRequest q)
        {
            using var c = new SqlConnection(_conn); using var cmd = Cmd(c, "PR_DGMS_SEC_CHECK_BLACKLIST");
            cmd.Parameters.AddWithValue("@IP_ID_TYPE", S(q.idType)); cmd.Parameters.AddWithValue("@IP_ID_NO", S(q.idNo)); cmd.Parameters.AddWithValue("@IP_NAME", S(q.name)); c.Open();
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return new SecBlacklistCheckResult { found = false };
            return new SecBlacklistCheckResult { found = true, matchType = Str(r, "MATCH_TYPE"), fullName = Str(r, "FULL_NM"), reasonCode = Str(r, "REASON_CD"), blacklistNo = Str(r, "BLACKLIST_NO") };
        }

        public (string status, string? message, SecIssuePassResult? data) IssuePass(SecIssuePassRequest q)
        {
            using var c = new SqlConnection(_conn); using var cmd = Cmd(c, "PR_DGMS_SEC_ISSUE_PASS");
            cmd.Parameters.AddWithValue("@IP_PASS_TYPE", S(q.passType)); cmd.Parameters.AddWithValue("@IP_BOOKING_ID", V(q.bookingId > 0 ? q.bookingId : null));
            cmd.Parameters.AddWithValue("@IP_VISITOR_NM", S(q.visitorName)); cmd.Parameters.AddWithValue("@IP_GENDER", S(q.gender));
            cmd.Parameters.AddWithValue("@IP_ID_TYPE", S(q.idType)); cmd.Parameters.AddWithValue("@IP_ID_NO", S(q.idNo)); cmd.Parameters.AddWithValue("@IP_MOBILE", S(q.mobile));
            cmd.Parameters.Add("@IP_VALID_FROM", SqlDbType.DateTime).Value = Utc(q.validFromUtc); cmd.Parameters.Add("@IP_VALID_TO", SqlDbType.DateTime).Value = Utc(q.validToUtc);
            cmd.Parameters.AddWithValue("@IP_ID_DOC_STORED", YN(q.idDocStored)); cmd.Parameters.AddWithValue("@IP_PHOTO_STORED", YN(q.photoStored)); cmd.Parameters.AddWithValue("@IP_PHOTO_URL", S(q.photoUrl));
            cmd.Parameters.AddWithValue("@IP_CONSENT_ID", YN(q.consentId)); cmd.Parameters.AddWithValue("@IP_CONSENT_VERIFY", YN(q.consentVerify)); cmd.Parameters.AddWithValue("@IP_REMARKS", S(q.remarks));
            var id = Out(cmd, "@OUT_PASS_ID", SqlDbType.Int); var no = Out(cmd, "@OUT_PASS_NO", SqlDbType.VarChar, 30); var tok = Out(cmd, "@OUT_TOKEN", SqlDbType.VarChar, 64);
            var st = Out(cmd, "@OUT_STATUS", SqlDbType.Char, 1); var msg = Out(cmd, "@OUT_MESSAGE", SqlDbType.VarChar, 500);
            c.Open(); cmd.ExecuteNonQuery();
            var status = Convert.ToString(st.Value) ?? "E"; var message = msg.Value as string;
            if (status != "S") return (status, message, null);
            var pn = no.Value as string; var t = tok.Value as string;
            return (status, message, new SecIssuePassResult { passId = id.Value is int i ? i : 0, passNo = pn, token = t, qrPayload = pn + "|" + t });
        }

        public List<SecPass> GetPasses(string? status, string? search, DateTime? fromUtc, DateTime? toUtc)
        {
            var list = new List<SecPass>();
            using var c = new SqlConnection(_conn); using var cmd = Cmd(c, "PR_DGMS_SEC_GET_PASSES");
            cmd.Parameters.AddWithValue("@IP_STATUS", S(status)); cmd.Parameters.AddWithValue("@IP_SEARCH", S(search));
            cmd.Parameters.AddWithValue("@IP_FROM_UTC", UtcN(fromUtc)); cmd.Parameters.AddWithValue("@IP_TO_UTC", UtcN(toUtc)); c.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(new SecPass
            {
                passId = Int(r, "PASS_ID"), passNo = Str(r, "PASS_NO") ?? "", passType = Str(r, "PASS_TYPE"), visitorName = Str(r, "VISITOR_NM"), mobile = Str(r, "VISITOR_MOBILE"),
                idType = Str(r, "VISITOR_ID_TYPE"), idMasked = Str(r, "VISITOR_ID_MASKED"), bookingId = NInt(r, "BOOKING_ID"), roomId = NInt(r, "ROOM_ID"), roomNo = Str(r, "ROOM_NO"),
                validFrom = Dt(r, "VALID_FROM_DT"), validTo = Dt(r, "VALID_TO_DT"), status = Str(r, "PASS_STATUS"), isInside = Bool(r, "IS_INSIDE"), revokeReason = Str(r, "REVOKE_REASON"),
                photoUrl = Str(r, "PHOTO_URL"), qrPayload = Str(r, "QR_PAYLOAD"), createdOn = Dt(r, "CREATE_DT")
            });
            return list;
        }

        public (string status, string? message) RevokePass(SecRevokeRequest q)
        {
            using var c = new SqlConnection(_conn); using var cmd = Cmd(c, "PR_DGMS_SEC_REVOKE_PASS");
            cmd.Parameters.AddWithValue("@IP_PASS_ID", q.passId); cmd.Parameters.AddWithValue("@IP_REASON", S(q.reason));
            var st = Out(cmd, "@OUT_STATUS", SqlDbType.Char, 1); var msg = Out(cmd, "@OUT_MESSAGE", SqlDbType.VarChar, 500);
            c.Open(); cmd.ExecuteNonQuery(); return (Convert.ToString(st.Value) ?? "E", msg.Value as string);
        }

        // ---------- scan / log ----------
        public SecScanResult Scan(SecScanRequest q, string? ip)
        {
            using var c = new SqlConnection(_conn); using var cmd = Cmd(c, "PR_DGMS_SEC_SCAN");
            cmd.Parameters.AddWithValue("@IP_PASS_NO", S(q.passNo)); cmd.Parameters.AddWithValue("@IP_TOKEN", S(q.token)); cmd.Parameters.AddWithValue("@IP_GATE", S(q.gate));
            cmd.Parameters.AddWithValue("@IP_ACTION", S(q.action)); cmd.Parameters.AddWithValue("@IP_DEVICE_ID", S(q.deviceId)); cmd.Parameters.AddWithValue("@IP_IP_ADDRESS", S(ip)); c.Open();
            using var r = cmd.ExecuteReader(); if (!r.Read()) throw new InvalidOperationException("Scan returned no result.");
            return new SecScanResult { allowed = Bool(r, "ALLOWED"), resultCode = Str(r, "RESULT_CD"), actionCode = Str(r, "ACTION_CD"), message = Str(r, "MESSAGE"), passNo = Str(r, "PASS_NO"), visitorName = Str(r, "VISITOR_NM"), roomNo = Str(r, "ROOM_NO"), validTo = Dt(r, "VALID_TO_DT") };
        }

        public SecLogPage GetEntryLog(DateTime fromUtc, DateTime toUtc, string? result, string? search, int page, int pageSize)
        {
            var o = new SecLogPage { page = page, pageSize = pageSize };
            using var c = new SqlConnection(_conn); using var cmd = Cmd(c, "PR_DGMS_SEC_GET_ENTRY_LOG");
            cmd.Parameters.AddWithValue("@IP_FROM_UTC", Utc(fromUtc)); cmd.Parameters.AddWithValue("@IP_TO_UTC", Utc(toUtc)); cmd.Parameters.AddWithValue("@IP_RESULT", S(result));
            cmd.Parameters.AddWithValue("@IP_SEARCH", S(search)); cmd.Parameters.AddWithValue("@IP_PAGE", page); cmd.Parameters.AddWithValue("@IP_PAGE_SIZE", pageSize); c.Open();
            using var r = cmd.ExecuteReader();
            if (r.Read()) o.summary = new SecLogSummary { entries = Int(r, "ENTRIES"), exits = Int(r, "EXITS"), denied = Int(r, "DENIED") };
            if (r.NextResult())
                while (r.Read())
                {
                    o.total = Int(r, "TOTAL_ROWS");
                    o.rows.Add(new SecLogRow { logId = Long(r, "LOG_ID"), logDt = Dt(r, "LOG_DT"), passNo = Str(r, "PASS_NO"), visitorName = Str(r, "VISITOR_NM"), roomNo = Str(r, "ROOM_NO"), gateCode = Str(r, "GATE_CD"), gateName = Str(r, "GATE_NM"), actionCode = Str(r, "ACTION_CD"), scanResult = Str(r, "SCAN_RESULT"), denyReason = Str(r, "DENY_REASON"), deviceId = Str(r, "DEVICE_ID") });
                }
            return o;
        }

        // ---------- blacklist ----------
        public List<SecBlacklistEntry> GetBlacklist(string? search, bool includeExpired)
        {
            var list = new List<SecBlacklistEntry>();
            using var c = new SqlConnection(_conn); using var cmd = Cmd(c, "PR_DGMS_SEC_GET_BLACKLIST");
            cmd.Parameters.AddWithValue("@IP_SEARCH", S(search)); cmd.Parameters.AddWithValue("@IP_INCLUDE_EXPIRED", includeExpired); c.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(new SecBlacklistEntry { blacklistId = Int(r, "BLACKLIST_ID"), blacklistNo = Str(r, "BLACKLIST_NO"), fullName = Str(r, "FULL_NM") ?? "", idType = Str(r, "ID_TYPE"), idMasked = Str(r, "ID_NO_MASKED"), reasonCode = Str(r, "REASON_CD"), reasonName = Str(r, "REASON_NM"), reasonDesc = Str(r, "REASON_DESC"), effectiveFrom = Dt(r, "EFFECTIVE_FROM_DT"), effectiveTo = Dt(r, "EFFECTIVE_TO_DT"), isLegalHold = Bool(r, "IS_LEGAL_HOLD"), lastReviewed = Dt(r, "LAST_REVIEWED_DT"), listStatus = Str(r, "LIST_STATUS") });
            return list;
        }

        public (string status, string? message, int id) SaveBlacklist(SecBlacklistSaveRequest q)
        {
            using var c = new SqlConnection(_conn); using var cmd = Cmd(c, "PR_DGMS_SEC_SAVE_BLACKLIST");
            cmd.Parameters.AddWithValue("@IP_MODE", q.mode); cmd.Parameters.AddWithValue("@IP_ID", V(q.id > 0 ? q.id : null)); cmd.Parameters.AddWithValue("@IP_FULL_NM", S(q.fullName));
            cmd.Parameters.AddWithValue("@IP_ID_TYPE", S(q.idType)); cmd.Parameters.AddWithValue("@IP_ID_NO", S(q.idNo)); cmd.Parameters.AddWithValue("@IP_REASON_CD", S(q.reasonCode));
            cmd.Parameters.AddWithValue("@IP_REASON_DESC", S(q.reasonDesc)); cmd.Parameters.AddWithValue("@IP_FROM_UTC", UtcN(q.fromUtc)); cmd.Parameters.AddWithValue("@IP_TO_UTC", UtcN(q.toUtc));
            var id = Out(cmd, "@OUT_ID", SqlDbType.Int); var st = Out(cmd, "@OUT_STATUS", SqlDbType.Char, 1); var msg = Out(cmd, "@OUT_MESSAGE", SqlDbType.VarChar, 500);
            c.Open(); cmd.ExecuteNonQuery(); return (Convert.ToString(st.Value) ?? "E", msg.Value as string, id.Value is int i ? i : 0);
        }

        // ---------- incidents ----------
        public List<SecIncident> GetIncidents(string? status, string? type, DateTime? fromUtc, DateTime? toUtc, string? search)
        {
            var list = new List<SecIncident>();
            using var c = new SqlConnection(_conn); using var cmd = Cmd(c, "PR_DGMS_SEC_GET_INCIDENTS");
            cmd.Parameters.AddWithValue("@IP_STATUS", S(status)); cmd.Parameters.AddWithValue("@IP_TYPE", S(type));
            cmd.Parameters.AddWithValue("@IP_FROM_UTC", UtcN(fromUtc)); cmd.Parameters.AddWithValue("@IP_TO_UTC", UtcN(toUtc)); cmd.Parameters.AddWithValue("@IP_SEARCH", S(search)); c.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(new SecIncident { incidentId = Int(r, "INCIDENT_ID"), incidentNo = Str(r, "INCIDENT_NO") ?? "", incidentType = Str(r, "INCIDENT_TYPE"), incidentTypeName = Str(r, "INCIDENT_TYPE_NM"), incidentDt = Dt(r, "INCIDENT_DT"), roomId = NInt(r, "ROOM_ID"), roomNo = Str(r, "ROOM_NO"), passId = NInt(r, "PASS_ID"), gate = Str(r, "ENTRY_POINT_CD"), description = Str(r, "DESCRIPTION"), actionTaken = Str(r, "ACTION_TAKEN"), resolution = Str(r, "RESOLUTION_REMARKS"), status = Str(r, "INCIDENT_STATUS"), closedDt = Dt(r, "CLOSED_DT"), createdOn = Dt(r, "CREATE_DT") });
            return list;
        }

        public (string status, string? message, int id, string? no) SaveIncident(SecIncidentSaveRequest q)
        {
            using var c = new SqlConnection(_conn); using var cmd = Cmd(c, "PR_DGMS_SEC_SAVE_INCIDENT");
            cmd.Parameters.AddWithValue("@IP_ID", V(q.id > 0 ? q.id : null)); cmd.Parameters.AddWithValue("@IP_TYPE", S(q.type)); cmd.Parameters.AddWithValue("@IP_INCIDENT_DT", UtcN(q.incidentUtc));
            cmd.Parameters.AddWithValue("@IP_ROOM_ID", V(q.roomId > 0 ? q.roomId : null)); cmd.Parameters.AddWithValue("@IP_GATE", S(q.gate)); cmd.Parameters.AddWithValue("@IP_DESCRIPTION", S(q.description) is DBNull ? "" : q.description.Trim());
            cmd.Parameters.AddWithValue("@IP_ACTION_TAKEN", S(q.actionTaken)); cmd.Parameters.AddWithValue("@IP_STATUS", S(q.status)); cmd.Parameters.AddWithValue("@IP_RESOLUTION", S(q.resolution));
            var id = Out(cmd, "@OUT_ID", SqlDbType.Int); var no = Out(cmd, "@OUT_INCIDENT_NO", SqlDbType.VarChar, 30); var st = Out(cmd, "@OUT_STATUS", SqlDbType.Char, 1); var msg = Out(cmd, "@OUT_MESSAGE", SqlDbType.VarChar, 500);
            c.Open(); cmd.ExecuteNonQuery(); return (Convert.ToString(st.Value) ?? "E", msg.Value as string, id.Value is int i ? i : 0, no.Value as string);
        }
    }
}
