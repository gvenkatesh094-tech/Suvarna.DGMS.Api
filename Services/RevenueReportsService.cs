using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class RevenueReportsService
    {
        private const int MaxDateRangeDays = 366;

        private readonly RevenueReportsRepository _repo;

        public RevenueReportsService(RevenueReportsRepository repo)
        {
            _repo = repo;
        }

        // ==========================================
        // KPI SUMMARY
        // ==========================================

        public ApiResponse<RevenueKpiSummary> GetKpiSummary(int sessionId, int? propertyId)
        {
            var data = _repo.GetKpiSummary(sessionId, propertyId);
            return ApiResponse<RevenueKpiSummary>.Success("Revenue KPI summary fetched successfully.", data);
        }

        // ==========================================
        // DAILY REVENUE
        // ==========================================

        public ApiResponse<List<RevenueDailyItem>> GetDailyRevenue(int sessionId, DateTime fromDt, DateTime toDt, int? propertyId)
        {
            ValidateDateRange(fromDt, toDt);

            var data = _repo.GetDailyRevenue(sessionId, fromDt, toDt, propertyId);

            return ApiResponse<List<RevenueDailyItem>>.Success(
                data.Count == 0 ? "No revenue data for the selected period." : "Daily revenue fetched successfully.",
                data
            );
        }

        // ==========================================
        // PAYMENT MODE SUMMARY
        // ==========================================

        public ApiResponse<List<RevenuePaymentModeItem>> GetPaymentModeSummary(int sessionId, DateTime fromDt, DateTime toDt)
        {
            ValidateDateRange(fromDt, toDt);

            var data = _repo.GetPaymentModeSummary(sessionId, fromDt, toDt);

            return ApiResponse<List<RevenuePaymentModeItem>>.Success(
                data.Count == 0 ? "No payments recorded for the selected period." : "Payment mode summary fetched successfully.",
                data
            );
        }

        // ==========================================
        // VALIDATION
        // ==========================================

        private static void ValidateDateRange(DateTime fromDt, DateTime toDt)
        {
            if (fromDt == default || toDt == default)
                throw new Exception("From and To dates are required.");

            if (fromDt.Date > toDt.Date)
                throw new Exception("From date cannot be after To date.");

            if ((toDt.Date - fromDt.Date).TotalDays > MaxDateRangeDays)
                throw new Exception($"Date range cannot exceed {MaxDateRangeDays} days.");
        }
    }
}
