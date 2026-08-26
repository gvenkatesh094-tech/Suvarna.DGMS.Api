using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class ExecutiveDashboardService
    {
        private readonly ExecutiveDashboardRepository _repo;

        public ExecutiveDashboardService(ExecutiveDashboardRepository repo)
        {
            _repo = repo;
        }

        // ==========================================
        // KPI SUMMARY
        // ==========================================

        public ApiResponse<ExecKpiSummary> GetKpiSummary(int sessionId, int? propertyId)
        {
            var data = _repo.GetKpiSummary(sessionId, propertyId);
            return ApiResponse<ExecKpiSummary>.Success("Executive KPI summary fetched successfully.", data);
        }

        // ==========================================
        // PROPERTY PERFORMANCE
        // ==========================================

        public ApiResponse<List<ExecPropertyPerformance>> GetPropertyPerformance(int sessionId)
        {
            var data = _repo.GetPropertyPerformance(sessionId);
            return ApiResponse<List<ExecPropertyPerformance>>.Success(
                data.Count == 0 ? "No properties configured." : "Property performance fetched successfully.",
                data
            );
        }

        // ==========================================
        // REVENUE TREND
        // ==========================================

        public ApiResponse<List<ExecRevenueTrendItem>> GetRevenueTrend(int sessionId, int? days)
        {
            var d = days ?? 7;
            if (d < 1 || d > 90) throw new Exception("Days must be between 1 and 90.");

            var data = _repo.GetRevenueTrend(sessionId, d);
            return ApiResponse<List<ExecRevenueTrendItem>>.Success("Revenue trend fetched successfully.", data);
        }

        // ==========================================
        // GST SLAB SUMMARY
        // ==========================================

        public ApiResponse<List<ExecGstSlabItem>> GetGstSlabSummary(int sessionId)
        {
            var data = _repo.GetGstSlabSummary(sessionId);
            return ApiResponse<List<ExecGstSlabItem>>.Success(
                data.Count == 0 ? "No GST activity this month." : "GST slab summary fetched successfully.",
                data
            );
        }

        // ==========================================
        // ALERTS
        // ==========================================

        public ApiResponse<List<ExecAlertItem>> GetAlerts(int sessionId, int? topN)
        {
            var n = topN ?? 10;
            if (n < 1 || n > 50) n = 10;

            var data = _repo.GetAlerts(sessionId, n);
            return ApiResponse<List<ExecAlertItem>>.Success(
                data.Count == 0 ? "No active alerts." : "Executive alerts fetched successfully.",
                data
            );
        }

        // ==========================================
        // OPERATIONAL SNAPSHOT
        // ==========================================

        public ApiResponse<ExecOperationalSnapshot> GetOperationalSnapshot(int sessionId)
        {
            var data = _repo.GetOperationalSnapshot(sessionId);
            return ApiResponse<ExecOperationalSnapshot>.Success("Operational snapshot fetched successfully.", data);
        }
    }
}
