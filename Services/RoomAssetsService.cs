using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class RoomAssetsService
    {
        private readonly RoomAssetsRepository _repo;

        public RoomAssetsService(RoomAssetsRepository repo)
        {
            _repo = repo;
        }

        public ApiResponse<List<RoomAssetRow>> GetAssets(int sessionId)
        {
            var data = _repo.GetAssets(sessionId);
            return ApiResponse<List<RoomAssetRow>>.Success("Room assets fetched successfully.", data);
        }

        public ApiResponse<List<AssetRoomLookup>> GetRooms(int sessionId)
        {
            var data = _repo.GetRooms(sessionId);
            return ApiResponse<List<AssetRoomLookup>>.Success("Rooms fetched successfully.", data);
        }

        public ApiResponse<SaveAssetResult> SaveAsset(SaveRoomAssetRequest request)
        {
            ValidateSave(request);

            var data = _repo.SaveAsset(request);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to save asset." : data.message);
            }

            return ApiResponse<SaveAssetResult>.Success(data.message, data);
        }

        public ApiResponse<AssetActionResult> LogRepair(LogAssetRepairRequest request)
        {
            var data = _repo.LogRepair(request);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to log repair." : data.message);
            }

            return ApiResponse<AssetActionResult>.Success(data.message, data);
        }

        public ApiResponse<List<AssetDuePmRow>> GetAssetsDueForPm(int sessionId)
        {
            var data = _repo.GetAssetsDueForPm(sessionId);
            return ApiResponse<List<AssetDuePmRow>>.Success("Assets due for PM fetched successfully.", data);
        }

        // ==========================================
        // SERVER VALIDATION
        // ==========================================

        private static readonly string[] ValidTypes = { "AC", "TV", "FRIDGE", "GEYSER", "FURNITURE", "ELECTRONICS" };
        private static readonly string[] ValidStatuses = { "ACTIVE", "UNDER_REPAIR", "DEFECTIVE" };

        private static void ValidateSave(SaveRoomAssetRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.assetNo))
            {
                throw new Exception("Asset Number is required.");
            }

            if (request.assetNo.Length > 30)
            {
                throw new Exception("Asset Number cannot exceed 30 characters.");
            }

            if (request.roomId <= 0)
            {
                throw new Exception("Room is required.");
            }

            if (string.IsNullOrWhiteSpace(request.assetType) || !ValidTypes.Contains(request.assetType))
            {
                throw new Exception("Please select a valid asset type.");
            }

            if (!string.IsNullOrWhiteSpace(request.assetStatus) && !ValidStatuses.Contains(request.assetStatus))
            {
                throw new Exception("Invalid asset status.");
            }

            if (request.purchaseCostAmt.HasValue && request.purchaseCostAmt.Value < 0)
            {
                throw new Exception("Purchase cost cannot be negative.");
            }

            if (request.purchaseDt.HasValue && request.purchaseDt.Value > DateTime.UtcNow)
            {
                throw new Exception("Purchase date cannot be in the future.");
            }

            if (request.purchaseDt.HasValue && request.warrantyExpiryDt.HasValue
                && request.warrantyExpiryDt.Value < request.purchaseDt.Value)
            {
                throw new Exception("Warranty expiry date cannot be before the purchase date.");
            }
        }
    }
}
