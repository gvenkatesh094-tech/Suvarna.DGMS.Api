using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class RefundManagementService
    {
        private readonly RefundManagementRepository _repo;

        public RefundManagementService(RefundManagementRepository repo)
        {
            _repo = repo;
        }

        public ApiResponse<List<RefundEligibleBooking>> GetEligibleBookings(int sessionId)
        {
            var data = _repo.GetEligibleBookings(sessionId);
            return ApiResponse<List<RefundEligibleBooking>>.Success("Eligible bookings fetched successfully.", data);
        }

        public ApiResponse<SubmitRefundResult> SubmitRequest(SubmitRefundRequest request)
        {
            ValidateSubmit(request);

            var data = _repo.SubmitRequest(request);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to submit refund request." : data.message);
            }

            return ApiResponse<SubmitRefundResult>.Success(data.message, data);
        }

        public ApiResponse<List<RefundRequestRow>> GetRequests(int sessionId, string? status)
        {
            var data = _repo.GetRequests(sessionId, status);
            return ApiResponse<List<RefundRequestRow>>.Success("Refund requests fetched successfully.", data);
        }

        public ApiResponse<RefundActionResult> Decide(DecideRefundRequest request)
        {
            if (request.decision != "APPROVE" && request.decision != "REJECT")
            {
                throw new Exception("Decision must be APPROVE or REJECT.");
            }

            if (request.decision == "REJECT" && string.IsNullOrWhiteSpace(request.remarks))
            {
                throw new Exception("A reason is required to reject a refund request.");
            }

            var data = _repo.Decide(request);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to record decision." : data.message);
            }

            return ApiResponse<RefundActionResult>.Success(data.message, data);
        }

        public ApiResponse<RefundActionResult> Complete(int sessionId, int requestId)
        {
            var data = _repo.Complete(sessionId, requestId);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to complete refund." : data.message);
            }

            return ApiResponse<RefundActionResult>.Success(data.message, data);
        }

        // ==========================================
        // SERVER VALIDATION
        // ==========================================

        private static void ValidateSubmit(SubmitRefundRequest request)
        {
            if (request.bookingId <= 0)
            {
                throw new Exception("Please select a booking.");
            }

            if (request.amount <= 0)
            {
                throw new Exception("Refund amount must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(request.mode))
            {
                throw new Exception("Refund mode is required.");
            }

            if ((request.mode == "BANK_TRANSFER" || request.mode == "UPI") && string.IsNullOrWhiteSpace(request.accountRef))
            {
                throw new Exception("Account/UPI details are required for this refund mode.");
            }

            if (string.IsNullOrWhiteSpace(request.reasonCd))
            {
                throw new Exception("Reason for refund is required.");
            }

            if (request.reasonCd == "OTHER" && string.IsNullOrWhiteSpace(request.remarks))
            {
                throw new Exception("Please specify a reason in remarks.");
            }
        }
    }
}
