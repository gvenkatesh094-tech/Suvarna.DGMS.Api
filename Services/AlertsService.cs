using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class AlertsService
    {
        private static readonly HashSet<string> Statuses = new(StringComparer.OrdinalIgnoreCase) { "ALL", "UNREAD", "READ" };
        private static readonly HashSet<string> Actions = new(StringComparer.OrdinalIgnoreCase) { "READ", "UNREAD", "READ_ALL" };

        private readonly AlertsRepository _repo;

        public AlertsService(AlertsRepository repo)
        {
            _repo = repo;
        }

        public ApiResponse<UserAlertList> GetAlerts(AlertListRequest req)
        {
            req.Status = Statuses.Contains(req.Status ?? "") ? req.Status!.ToUpperInvariant() : "ALL";
            req.Show = string.Equals(req.Show, "OPEN", StringComparison.OrdinalIgnoreCase) ? "OPEN" : "ALL";
            req.PageNo = req.PageNo < 1 ? 1 : req.PageNo;
            req.PageSize = req.PageSize < 1 ? 20 : Math.Min(req.PageSize, 200);

            var data = _repo.GetAlerts(req);
            return ApiResponse<UserAlertList>.Success(
                data.Items.Count == 0 ? "No alerts." : "Alerts fetched successfully.",
                data
            );
        }

        public ApiResponse<AlertReadResult> SetRead(AlertReadRequest req)
        {
            if (!Actions.Contains(req.Action ?? ""))
                return ApiResponse<AlertReadResult>.Fail("Unknown action.", "400");

            if (!string.Equals(req.Action, "READ_ALL", StringComparison.OrdinalIgnoreCase) && (req.AlertId ?? 0) <= 0)
                return ApiResponse<AlertReadResult>.Fail("Alert is required.", "400");

            var (ok, message, updated) = _repo.SetRead(req);

            return ok
                ? ApiResponse<AlertReadResult>.Success(message, new AlertReadResult { Updated = updated })
                : ApiResponse<AlertReadResult>.Fail(message);
        }
    }
}
