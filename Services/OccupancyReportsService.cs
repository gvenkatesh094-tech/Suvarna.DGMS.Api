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

        public ApiResponse<List<OccRoomTypeStat>> GetByRoomType(int sessionId)
        {
            var data = _repo.GetByRoomType(sessionId);
            return ApiResponse<List<OccRoomTypeStat>>.Success(
                data.Count == 0 ? "No room types configured." : "Occupancy by room type fetched successfully.",
                data
            );
        }

        public ApiResponse<List<OccCategoryStat>> GetCategoryBreakdown(int sessionId)
        {
            var data = _repo.GetCategoryBreakdown(sessionId);
            return ApiResponse<List<OccCategoryStat>>.Success(
                data.Count == 0 ? "No occupant category activity this month." : "Category breakdown fetched successfully.",
                data
            );
        }

        public ApiResponse<List<OccTrendItem>> GetTrend(int sessionId, int? days)
        {
            var d = days ?? 7;
            if (d < 1 || d > 90) throw new Exception("Days must be between 1 and 90.");

            var data = _repo.GetTrend(sessionId, d);
            return ApiResponse<List<OccTrendItem>>.Success("Occupancy trend fetched successfully.", data);
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
