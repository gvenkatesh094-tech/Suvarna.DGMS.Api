using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class SecurityService
    {
        private readonly SecurityRepository _repo;
        public SecurityService(SecurityRepository repo) { _repo = repo; }

        public ApiResponse<List<SecLookupItem>> Lookups() => ApiResponse<List<SecLookupItem>>.Success("OK", _repo.GetLookups());
        public ApiResponse<List<SecHost>> Hosts(string? s) => ApiResponse<List<SecHost>>.Success("OK", _repo.GetHosts(s));

        public ApiResponse<SecKpis> Kpis(DateTime fromUtc, DateTime toUtc)
            => ValidRange(fromUtc, toUtc, 2) is string e ? ApiResponse<SecKpis>.Fail(e) : ApiResponse<SecKpis>.Success("OK", _repo.GetKpis(fromUtc, toUtc));

        public ApiResponse<SecBlacklistCheckResult> CheckBlacklist(SecBlacklistCheckRequest q)
            => string.IsNullOrWhiteSpace(q.idNo) ? ApiResponse<SecBlacklistCheckResult>.Success("OK", new SecBlacklistCheckResult()) : ApiResponse<SecBlacklistCheckResult>.Success("OK", _repo.CheckBlacklist(q));

        public ApiResponse<SecIssuePassResult> IssuePass(SecIssuePassRequest q)
        {
            if (string.IsNullOrWhiteSpace(q.visitorName)) return ApiResponse<SecIssuePassResult>.Fail("Visitor name is required.");
            if (string.IsNullOrWhiteSpace(q.passType)) return ApiResponse<SecIssuePassResult>.Fail("Pass type is required.");
            if (string.IsNullOrWhiteSpace(q.idType) || string.IsNullOrWhiteSpace(q.idNo)) return ApiResponse<SecIssuePassResult>.Fail("ID type and number are required.");
            if (q.validToUtc <= q.validFromUtc) return ApiResponse<SecIssuePassResult>.Fail("'Valid until' must be after 'Valid from'.");
            var (st, msg, data) = _repo.IssuePass(q);
            return st == "S" ? ApiResponse<SecIssuePassResult>.Success(msg ?? "Pass issued.", data!) : ApiResponse<SecIssuePassResult>.Fail(msg ?? "Could not issue the pass.");
        }

        public ApiResponse<List<SecPass>> Passes(string? status, string? search, DateTime? f, DateTime? t) => ApiResponse<List<SecPass>>.Success("OK", _repo.GetPasses(status, search, f, t));

        public ApiResponse<string> Revoke(SecRevokeRequest q)
        {
            if (q.passId <= 0) return ApiResponse<string>.Fail("Pass is required.");
            if (string.IsNullOrWhiteSpace(q.reason)) return ApiResponse<string>.Fail("Reason is required to revoke a pass.");
            var (st, msg) = _repo.RevokePass(q);
            return st == "S" ? ApiResponse<string>.Success(msg ?? "Pass revoked.", "OK") : ApiResponse<string>.Fail(msg ?? "Could not revoke the pass.");
        }

        public ApiResponse<SecScanResult> Scan(SecScanRequest q, string? ip)
        {
            if (string.IsNullOrWhiteSpace(q.passNo)) return ApiResponse<SecScanResult>.Fail("Pass number is required.");
            if (string.IsNullOrWhiteSpace(q.gate)) return ApiResponse<SecScanResult>.Fail("Select a gate.");
            if (!string.IsNullOrEmpty(q.action) && q.action != "E" && q.action != "X") return ApiResponse<SecScanResult>.Fail("Invalid action.");
            // a denied scan is a normal business outcome: Status=true, data.allowed=false (so the guard sees the reason, not an error toast)
            return ApiResponse<SecScanResult>.Success("OK", _repo.Scan(q, ip));
        }

        public ApiResponse<SecLogPage> EntryLog(DateTime f, DateTime t, string? result, string? search, int page, int size)
        {
            if (ValidRange(f, t, 93) is string e) return ApiResponse<SecLogPage>.Fail(e);
            return ApiResponse<SecLogPage>.Success("OK", _repo.GetEntryLog(f, t, result, search, Math.Max(page, 1), Math.Clamp(size, 1, 200)));
        }

        public ApiResponse<List<SecBlacklistEntry>> Blacklist(string? s, bool inc) => ApiResponse<List<SecBlacklistEntry>>.Success("OK", _repo.GetBlacklist(s, inc));

        public ApiResponse<SecSaveResult> SaveBlacklist(SecBlacklistSaveRequest q)
        {
            if (q.mode is not ("I" or "U" or "R" or "V")) return ApiResponse<SecSaveResult>.Fail("Invalid mode.");
            var (st, msg, id) = _repo.SaveBlacklist(q);
            return st == "S" ? ApiResponse<SecSaveResult>.Success(msg ?? "Saved.", new SecSaveResult { id = id }) : ApiResponse<SecSaveResult>.Fail(msg ?? "Could not save.");
        }

        public ApiResponse<List<SecIncident>> Incidents(string? status, string? type, DateTime? f, DateTime? t, string? s) => ApiResponse<List<SecIncident>>.Success("OK", _repo.GetIncidents(status, type, f, t, s));

        public ApiResponse<SecSaveResult> SaveIncident(SecIncidentSaveRequest q)
        {
            var (st, msg, id, no) = _repo.SaveIncident(q);
            return st == "S" ? ApiResponse<SecSaveResult>.Success(msg ?? "Saved.", new SecSaveResult { id = id, number = no }) : ApiResponse<SecSaveResult>.Fail(msg ?? "Could not save.");
        }

        private static string? ValidRange(DateTime f, DateTime t, int maxDays)
            => t <= f ? "Invalid date range." : (t - f).TotalDays > maxDays ? $"Date range is too large (max {maxDays} days)." : null;
    }
}
