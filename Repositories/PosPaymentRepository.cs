using System.Data;
using System.Text.RegularExpressions;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Suvarna.DGMS.Api.Models;

namespace Suvarna.DGMS.Api.Repositories
{
    /// <summary>
    /// NEW: PayLink / Pine Labs POS payments (tables DGM_POS_TXN, DGM_POS_API_LOG).
    /// Every procedure resolves group / org / location / user from @IP_SESSION_ID.
    /// </summary>
    public class PosPaymentRepository
    {
        private readonly string _connectionString;

        public PosPaymentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SqlServer")
                ?? throw new InvalidOperationException("Connection string 'SqlServer' is missing.");
        }

        // =====================================================
        // POS MACHINES OF THE LOGIN SESSION
        // =====================================================

        public async Task<List<PineLabMissionData>> GetPineLabMissionDataAsync(string sessionId)
        {
            // the shared HIMS procedure (also used by another project): only called, never changed
            const string procedure = "INT_PR_GET_PINE_LAB_MISSION_DATA";

            using var con = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@IP_SESSION_ID", int.TryParse(sessionId, out var sid) ? sid : 0, DbType.Int32);

            var result = await con.QueryAsync<PineLabMissionData>(
                procedure,
                parameters,
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        // =====================================================
        // CREATE
        // =====================================================

        public async Task<(long posTxnId, string txnNo, string status, string message, long blockingPosTxnId)> CreateAsync(
            string sessionId, PosInitiateRequest request, string txnType, PineLabMissionData device)
        {
            using var con = new SqlConnection(_connectionString);

            var p = new DynamicParameters();
            p.Add("@IP_SESSION_ID", sessionId);
            p.Add("@IP_PURPOSE", request.purpose);
            p.Add("@IP_REF_ID", request.refId);
            p.Add("@IP_REF_NO", Trim(request.refNo, 50));
            p.Add("@IP_PAYMENT_MODE", request.paymentMode);
            p.Add("@IP_TXN_TYPE", txnType);
            p.Add("@IP_AMOUNT", request.amount, DbType.Decimal, precision: 18, scale: 2);
            p.Add("@IP_DEVICE_CONFIG_ID", device.CONFIG_ID);
            p.Add("@IP_DEVICE_NO", Trim(device.DEVICE_NO, 50));
            p.Add("@IP_TERMINAL_NM", Trim(device.MAP_TERMINAL_NAME, 100));
            p.Add("@IP_CUSTOMER_NM", Trim(request.customerName, 150));
            p.Add("@IP_MOBILE_NO", Trim(request.mobileNo, 20));
            p.Add("@OUT_POS_TXN_ID", dbType: DbType.Int64, direction: ParameterDirection.Output);
            p.Add("@OUT_TXN_NO", dbType: DbType.AnsiString, size: 40, direction: ParameterDirection.Output);
            p.Add("@OUT_STATUS", dbType: DbType.AnsiStringFixedLength, size: 1, direction: ParameterDirection.Output);
            p.Add("@OUT_MESSAGE", dbType: DbType.AnsiString, size: 500, direction: ParameterDirection.Output);
            p.Add("@OUT_BLOCKING_POS_TXN_ID", dbType: DbType.Int64, direction: ParameterDirection.Output);

            await con.ExecuteAsync("DGMS.PR_DGMS_POS_TXN_CREATE", p, commandType: CommandType.StoredProcedure);

            return (p.Get<long?>("@OUT_POS_TXN_ID") ?? 0,
                    p.Get<string?>("@OUT_TXN_NO") ?? "",
                    p.Get<string?>("@OUT_STATUS") ?? "E",
                    p.Get<string?>("@OUT_MESSAGE") ?? "",
                    p.Get<long?>("@OUT_BLOCKING_POS_TXN_ID") ?? 0);
        }

        // =====================================================
        // UPDATE (push / status / close result)
        // =====================================================

        public async Task<(string status, string message)> UpdateAsync(
            string sessionId, long posTxnId, string newStatus,
            string? plTransactionId = null, string? plReferenceId = null,
            string? rrn = null, string? cardMasked = null, string? cardLast4 = null, string? cardHolder = null,
            decimal? paidAmount = null, string? message = null, bool isStatusCheck = false, string? closeReason = null)
        {
            using var con = new SqlConnection(_connectionString);

            var p = new DynamicParameters();
            p.Add("@IP_SESSION_ID", sessionId);
            p.Add("@IP_POS_TXN_ID", posTxnId);
            p.Add("@IP_NEW_STATUS", newStatus);
            p.Add("@IP_PL_TRANSACTION_ID", Trim(plTransactionId, 100));
            p.Add("@IP_PL_REFERENCE_ID", Trim(plReferenceId, 100));
            p.Add("@IP_RRN", Trim(rrn, 50));
            p.Add("@IP_CARD_MASKED", Trim(cardMasked, 30));
            p.Add("@IP_CARD_LAST4", Trim(cardLast4, 4));
            p.Add("@IP_CARD_HOLDER_NM", Trim(cardHolder, 100));
            p.Add("@IP_PAID_AMOUNT", paidAmount, DbType.Decimal, precision: 18, scale: 2);
            p.Add("@IP_MESSAGE", Trim(message, 500));
            p.Add("@IP_IS_STATUS_CHECK", isStatusCheck);
            p.Add("@IP_CLOSE_REASON", Trim(closeReason, 300));
            p.Add("@OUT_STATUS", dbType: DbType.AnsiStringFixedLength, size: 1, direction: ParameterDirection.Output);
            p.Add("@OUT_MESSAGE", dbType: DbType.AnsiString, size: 500, direction: ParameterDirection.Output);

            await con.ExecuteAsync("DGMS.PR_DGMS_POS_TXN_UPDATE", p, commandType: CommandType.StoredProcedure);

            return (p.Get<string?>("@OUT_STATUS") ?? "E", p.Get<string?>("@OUT_MESSAGE") ?? "");
        }

        // =====================================================
        // READ
        // =====================================================

        public async Task<PosTxnRow?> GetAsync(string sessionId, long posTxnId)
        {
            using var con = new SqlConnection(_connectionString);
            return await con.QueryFirstOrDefaultAsync<PosTxnRow>(
                "DGMS.PR_DGMS_POS_TXN_GET",
                new { IP_SESSION_ID = sessionId, IP_POS_TXN_ID = posTxnId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<PosTxnRow?> GetOpenAsync(string sessionId, string purpose, int refId, string? refNo)
        {
            using var con = new SqlConnection(_connectionString);
            return await con.QueryFirstOrDefaultAsync<PosTxnRow>(
                "DGMS.PR_DGMS_POS_TXN_GET_OPEN",
                new { IP_SESSION_ID = sessionId, IP_PURPOSE = purpose, IP_REF_ID = refId, IP_REF_NO = Trim(refNo, 50) },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<List<PosTxnRow>> ListAsync(string sessionId, DateTime fromDt, DateTime toDt, string? filter)
        {
            using var con = new SqlConnection(_connectionString);
            var rows = await con.QueryAsync<PosTxnRow>(
                "DGMS.PR_DGMS_POS_TXN_LIST",
                new { IP_SESSION_ID = sessionId, IP_FROM_DT = fromDt.Date, IP_TO_DT = toDt.Date, IP_FILTER = filter ?? "ALL" },
                commandType: CommandType.StoredProcedure);
            return rows.ToList();
        }

        public async Task<List<PosApiLogRow>> GetLogsAsync(string sessionId, long posTxnId)
        {
            using var con = new SqlConnection(_connectionString);
            var rows = await con.QueryAsync<PosApiLogRow>(
                "DGMS.PR_DGMS_POS_LOG_GET",
                new { IP_SESSION_ID = sessionId, IP_POS_TXN_ID = posTxnId },
                commandType: CommandType.StoredProcedure);
            return rows.ToList();
        }

        // =====================================================
        // CLAIM / RELEASE / LINK  (sync: called from the existing sync payment services)
        // =====================================================

        public (PosClaim? claim, string message) Claim(string sessionId, long posTxnId, string purpose, int refId, decimal amount)
        {
            using var con = new SqlConnection(_connectionString);

            var p = new DynamicParameters();
            p.Add("@IP_SESSION_ID", sessionId);
            p.Add("@IP_POS_TXN_ID", posTxnId);
            p.Add("@IP_PURPOSE", purpose);
            p.Add("@IP_REF_ID", refId);
            p.Add("@IP_AMOUNT", amount, DbType.Decimal, precision: 18, scale: 2);
            p.Add("@OUT_CLAIM_TOKEN", dbType: DbType.Guid, direction: ParameterDirection.Output);
            p.Add("@OUT_STATUS", dbType: DbType.AnsiStringFixedLength, size: 1, direction: ParameterDirection.Output);
            p.Add("@OUT_MESSAGE", dbType: DbType.AnsiString, size: 500, direction: ParameterDirection.Output);

            var row = con.QueryFirstOrDefault<PosTxnRow>(
                "DGMS.PR_DGMS_POS_TXN_CLAIM", p, commandType: CommandType.StoredProcedure);

            var status = p.Get<string?>("@OUT_STATUS") ?? "E";
            var message = p.Get<string?>("@OUT_MESSAGE") ?? "";
            var token = p.Get<Guid?>("@OUT_CLAIM_TOKEN");

            if (status != "S" || token == null || row == null)
                return (null, string.IsNullOrWhiteSpace(message) ? "POS payment could not be used." : message);

            return (new PosClaim
            {
                PosTxnId = row.POS_TXN_ID,
                Token = token.Value,
                TxnNo = row.TXN_NO ?? "",
                Rrn = row.RRN ?? "",
                CardLast4 = row.CARD_LAST4,
                CardMasked = row.CARD_MASKED,
                CardHolderName = row.CARD_HOLDER_NM,
                Amount = row.AMOUNT,
                PaymentMode = row.PAYMENT_MODE
            }, message);
        }

        public void Release(string sessionId, PosClaim claim)
        {
            using var con = new SqlConnection(_connectionString);
            con.Execute("DGMS.PR_DGMS_POS_TXN_RELEASE",
                new { IP_SESSION_ID = sessionId, IP_POS_TXN_ID = claim.PosTxnId, IP_CLAIM_TOKEN = claim.Token },
                commandType: CommandType.StoredProcedure);
        }

        public (string status, string message) Link(string sessionId, PosClaim claim, int? bookingId, int? billId, int? paymentId)
        {
            using var con = new SqlConnection(_connectionString);

            var p = new DynamicParameters();
            p.Add("@IP_SESSION_ID", sessionId);
            p.Add("@IP_POS_TXN_ID", claim.PosTxnId);
            p.Add("@IP_CLAIM_TOKEN", claim.Token);
            p.Add("@IP_BOOKING_ID", bookingId);
            p.Add("@IP_BILL_ID", billId);
            p.Add("@IP_PAYMENT_ID", paymentId);
            p.Add("@OUT_STATUS", dbType: DbType.AnsiStringFixedLength, size: 1, direction: ParameterDirection.Output);
            p.Add("@OUT_MESSAGE", dbType: DbType.AnsiString, size: 500, direction: ParameterDirection.Output);

            con.Execute("DGMS.PR_DGMS_POS_TXN_LINK", p, commandType: CommandType.StoredProcedure);

            return (p.Get<string?>("@OUT_STATUS") ?? "E", p.Get<string?>("@OUT_MESSAGE") ?? "");
        }

        // =====================================================
        // API LOG  (never throws: a log problem must not break a payment)
        // =====================================================

        public async Task<bool> InsertLogAsync(
            long? posTxnId, string? txnNo, string apiName, string method, string url,
            string? requestBody, string? responseBody, int? httpStatus, bool isSuccess, string? error,
            int durationMs, string? correlationId, string? sessionId, int? userId)
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                await con.ExecuteAsync("DGMS.PR_DGMS_POS_LOG_INSERT", new
                {
                    IP_POS_TXN_ID = posTxnId,
                    IP_TXN_NO = txnNo,
                    IP_API_NAME = apiName,
                    IP_HTTP_METHOD = method,
                    IP_REQUEST_URL = url,
                    IP_REQUEST_BODY = requestBody,
                    IP_RESPONSE_BODY = responseBody,
                    IP_HTTP_STATUS = httpStatus,
                    IP_IS_SUCCESS = isSuccess ? "Y" : "N",
                    IP_ERROR_MSG = Trim(error, 1000),
                    IP_DURATION_MS = durationMs,
                    IP_CORRELATION_ID = Trim(correlationId, 64),
                    IP_SESSION_ID = sessionId,
                    IP_USER_ID = userId
                }, commandType: CommandType.StoredProcedure);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string? Trim(string? value, int max)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            var v = value.Trim();
            return v.Length > max ? v[..max] : v;
        }
    }
}
