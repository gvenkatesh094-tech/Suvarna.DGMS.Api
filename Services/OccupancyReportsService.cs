using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class OccupancyReportsService
    {
        private readonly OccupancyReportsRepository _repo;

        public OccupancyReportsService(OccupancyReportsRepository repo)
        {
            _repo = repo;
        }

        public ApiResponse<OccKpiSummary> GetKpiSummary(int sessionId, int? propertyId)
        {
            var data = _repo.GetKpiSummary(sessionId, propertyId);
            return ApiResponse<OccKpiSummary>.Success("Occupancy KPI summary fetched successfully.", data);
        }

        public ApiResponse<List<OccRoomTypeStat>> GetByRoomType(int sessionId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            ValidatePeriod(fromDate, toDate);
            var data = _repo.GetByRoomType(sessionId, fromDate, toDate);
            return ApiResponse<List<OccRoomTypeStat>>.Success(
                data.Count == 0 ? "No room types configured." : "Occupancy by room type fetched successfully.",
                data
            );
        }

        public ApiResponse<List<OccCategoryStat>> GetCategoryBreakdown(int sessionId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            ValidatePeriod(fromDate, toDate);
            var data = _repo.GetCategoryBreakdown(sessionId, fromDate, toDate);
            return ApiResponse<List<OccCategoryStat>>.Success(
                data.Count == 0 ? "No occupant category activity in this period." : "Category breakdown fetched successfully.",
                data
            );
        }

        public ApiResponse<List<OccTrendItem>> GetTrend(int sessionId, int? days)
        {
            var d = days ?? 7;
            if (d < 1 || d > 90) throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Days must be between 1 and 90.");

            var data = _repo.GetTrend(sessionId, d);
            return ApiResponse<List<OccTrendItem>>.Success("Occupancy trend fetched successfully.", data);
        }

        // NEW: period of the report (both optional; default month to date)
        private static void ValidatePeriod(DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate.HasValue && toDate.HasValue)
            {
                if (fromDate.Value.Date > toDate.Value.Date)
                    throw new Suvarna.DGMS.Api.Helpers.UserMessageException("From date cannot be after To date.");
                if ((toDate.Value.Date - fromDate.Value.Date).TotalDays > 366)
                    throw new Suvarna.DGMS.Api.Helpers.UserMessageException("The period cannot be longer than one year.");
            }
        }

        public ApiResponse<List<OccRoomStatusItem>> GetRoomStatusGrid(int sessionId, int? propertyId)
        {
            var data = _repo.GetRoomStatusGrid(sessionId, propertyId);
            return ApiResponse<List<OccRoomStatusItem>>.Success(
                data.Count == 0 ? "No rooms configured." : "Room status grid fetched successfully.",
                data
            );
        }
    }
}
