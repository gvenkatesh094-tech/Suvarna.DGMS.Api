using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class DepositLedgerService
    {
        // FIX: one refund-mode list for the whole system — same as
        // CheckoutService.AllowedRefundModes and PR_DGMS_REFUND_DEPOSIT.
        public static readonly string[] ValidRefundModes =
            { "CASH", "UPI", "BANK_TRANSFER", "CARD_REVERSAL", "CHEQUE" };

        private readonly DepositLedgerRepository _repo;

        public DepositLedgerService(DepositLedgerRepository repo)
        {
            _repo = repo;
        }

        // ==========================================
        // GET DEPOSIT LEDGER
        // ==========================================

        public ApiResponse<List<DepositLedgerBooking>> GetDepositLedger(int sessionId, int? propertyId, string? statusFilter)
        {
            if (!string.IsNullOrWhiteSpace(statusFilter) &&
                !new[] { "ALL", "HELD", "EXHAUSTED" }.Contains(statusFilter.Trim().ToUpperInvariant()))
            {
                throw new Exception("Invalid status filter. Use HELD, EXHAUSTED or ALL.");
            }

            var data = _repo.GetDepositLedger(sessionId, propertyId, statusFilter);

            return ApiResponse<List<DepositLedgerBooking>>.Success(
                data.Count == 0 ? "No deposit-holding bookings found." : "Deposit ledger fetched successfully.",
                data);
        }

        // ==========================================
        // GET DEPOSIT TRANSACTION HISTORY
        // ==========================================

        public ApiResponse<List<DepositTxnHistory>> GetDepositTxnHistory(int sessionId, int bookingId)
        {
            if (bookingId <= 0) throw new Exception("A valid booking is required.");

            var data = _repo.GetDepositTxnHistory(sessionId, bookingId);

            return ApiResponse<List<DepositTxnHistory>>.Success(
                data.Count == 0 ? "No deposit transactions found for this booking." : "Deposit transaction history fetched successfully.",
                data);
        }

        // ==========================================
        // REFUND DEPOSIT
        // ==========================================

        public ApiResponse<DepositRefundResult> RefundDeposit(DepositRefundRequest request)
        {
            ValidateRefundRequest(request);

            var data = _repo.RefundDeposit(request);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Deposit refund failed." : data.message);
            }

            return ApiResponse<DepositRefundResult>.Success(data.message, data);
        }

        // ==========================================
        // VALIDATION
        // (the refundable limit itself is enforced in the SP, because it
        //  depends on live charges / payments)
        // ==========================================

        private static void ValidateRefundRequest(DepositRefundRequest request)
        {
            if (request == null) throw new Exception("Refund request payload is required.");
            if (request.bookingId <= 0) throw new Exception("A valid booking is required.");
            if (request.refundAmount <= 0) throw new Exception("Refund amount must be greater than zero.");
            if (decimal.Round(request.refundAmount, 2) != request.refundAmount)
                throw new Exception("Refund amount can have at most 2 decimal places.");

            if (string.IsNullOrWhiteSpace(request.refundMode)) throw new Exception("Refund mode is required.");

            string mode = request.refundMode.Trim().ToUpperInvariant();
            if (!ValidRefundModes.Contains(mode))
                throw new Exception("Invalid refund mode. Allowed: " + string.Join(", ", ValidRefundModes) + ".");

            if (mode != "CASH" && string.IsNullOrWhiteSpace(request.refundRef))
                throw new Exception("Refund reference is required for non-cash refunds.");

            if (!string.IsNullOrWhiteSpace(request.refundRef) && request.refundRef.Trim().Length > 50)
                throw new Exception("Refund reference cannot exceed 50 characters.");

            if (!string.IsNullOrWhiteSpace(request.remarks) && request.remarks.Trim().Length > 300)
                throw new Exception("Remarks cannot exceed 300 characters.");
        }
    }
}
