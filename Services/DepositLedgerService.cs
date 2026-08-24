using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class DepositLedgerService
    {
        private static readonly string[] ValidRefundModes =
            { "CASH", "CARD", "UPI", "BANK_TRANSFER", "CHEQUE" };

        private readonly DepositLedgerRepository _repo;

        public DepositLedgerService(DepositLedgerRepository repo)
        {
            _repo = repo;
        }

        // ==========================================
        // GET DEPOSIT LEDGER
        // ==========================================

        public ApiResponse<List<DepositLedgerBooking>> GetDepositLedger(
            int sessionId,
            int? propertyId,
            string? statusFilter)
        {
            if (!string.IsNullOrWhiteSpace(statusFilter) &&
                !string.Equals(statusFilter, "ALL", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(statusFilter, "HELD", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(statusFilter, "EXHAUSTED", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Invalid status filter. Use HELD, EXHAUSTED or ALL.");
            }

            var data = _repo.GetDepositLedger(sessionId, propertyId, statusFilter);

            return ApiResponse<List<DepositLedgerBooking>>.Success(
                data.Count == 0
                    ? "No deposit-holding bookings found."
                    : "Deposit ledger fetched successfully.",
                data
            );
        }

        // ==========================================
        // GET DEPOSIT TRANSACTION HISTORY
        // ==========================================

        public ApiResponse<List<DepositTxnHistory>> GetDepositTxnHistory(int sessionId, int bookingId)
        {
            if (bookingId <= 0)
            {
                throw new Exception("A valid booking is required.");
            }

            var data = _repo.GetDepositTxnHistory(sessionId, bookingId);

            return ApiResponse<List<DepositTxnHistory>>.Success(
                data.Count == 0
                    ? "No deposit transactions found for this booking."
                    : "Deposit transaction history fetched successfully.",
                data
            );
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
                throw new Exception(
                    string.IsNullOrWhiteSpace(data.message)
                        ? "Deposit refund failed."
                        : data.message
                );
            }

            return ApiResponse<DepositRefundResult>.Success(data.message, data);
        }

        // ==========================================
        // VALIDATION
        // ==========================================

        private static void ValidateRefundRequest(DepositRefundRequest request)
        {
            if (request == null)
                throw new Exception("Refund request payload is required.");

            if (request.bookingId <= 0)
                throw new Exception("A valid booking is required.");

            if (request.refundAmount <= 0)
                throw new Exception("Refund amount must be greater than zero.");

            if (string.IsNullOrWhiteSpace(request.refundMode))
                throw new Exception("Refund mode is required.");

            if (!ValidRefundModes.Contains(request.refundMode.Trim().ToUpperInvariant()))
                throw new Exception("Invalid refund mode. Allowed: CASH, CARD, UPI, BANK_TRANSFER, CHEQUE.");

            if (!string.Equals(request.refundMode.Trim(), "CASH", StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrWhiteSpace(request.refundRef))
            {
                throw new Exception($"Refund reference is required for {request.refundMode} refunds.");
            }
        }
    }
}
