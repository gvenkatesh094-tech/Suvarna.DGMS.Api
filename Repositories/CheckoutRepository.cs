using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class CheckoutRepository
    {
        private readonly string _conn;

        public CheckoutRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        //=========================================================
        // GET CHECKOUT BOOKINGS
        //=========================================================

        public List<CheckoutBooking> GetCheckoutBookings(
            int sessionId,
            string? searchTerm = null)
        {
            List<CheckoutBooking> list = new List<CheckoutBooking>();

            using SqlConnection con = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_CHECKOUT_BOOKINGS", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
            cmd.Parameters.AddWithValue("@IP_SEARCH_TERM", string.IsNullOrWhiteSpace(searchTerm) ? DBNull.Value : searchTerm.Trim());

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new CheckoutBooking
                {
                    bookingId = GetInt(reader, "BOOKING_ID"),
                    bookingNo = GetString(reader, "BOOKING_NO"),
                    guestName = GetString(reader, "GUEST_NM"),
                    mobileNo = GetString(reader, "MOBILE_NO"),
                    roomId = GetInt(reader, "ROOM_ID"),
                    roomNo = GetString(reader, "ROOM_NO"),
                    roomName = GetString(reader, "ROOM_NM"),
                    checkinDate = Convert.ToDateTime(reader["ACTUAL_CHECKIN_DT"]),
                    expectedCheckoutDate = Convert.ToDateTime(reader["EXPECTED_CHECKOUT_DT"]),
                    stayDays = GetInt(reader, "STAY_DAYS"),
                    totalBill = GetDecimal(reader, "TOTAL_BILL"),
                    deposit = GetDecimal(reader, "DEPOSIT"),
                    paidAmount = GetDecimal(reader, "PAID_AMOUNT"),
                    balanceAmount = GetDecimal(reader, "BALANCE_AMOUNT"),
                    bookingStatus = GetString(reader, "BOOKING_STATUS"),
                    checkoutStatus = GetString(reader, "CHECKOUT_STATUS")
                });
            }

            return list;
        }

        //=========================================================
        // PREPARE / GET CHECKOUT DETAILS
        //=========================================================

        public CheckoutDetails? PrepareCheckout(int sessionId, int bookingId, DateTime checkoutDt)
        {
            return GetCheckoutDetails(sessionId, bookingId, checkoutDt);
        }

        public CheckoutDetails? GetCheckoutDetails(int sessionId, int bookingId, DateTime? checkoutDt)
        {
            CheckoutDetails? details = null;

            using SqlConnection con = new SqlConnection(_conn);
            con.Open();

            using (SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_CHECKOUT_DETAILS", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_BOOKING_ID", bookingId);
                cmd.Parameters.AddWithValue("@IP_CHECKOUT_DT", (object?)checkoutDt ?? DBNull.Value);

                using SqlDataReader reader = cmd.ExecuteReader();

                //=========================================
                // Result Set - 1 (Header Details)
                //=========================================

                if (reader.Read())
                {
                    details = new CheckoutDetails
                    {
                        bookingId = GetInt(reader, "BOOKING_ID"),
                        bookingNo = GetString(reader, "BOOKING_NO"),
                        bookingStatus = GetString(reader, "BOOKING_STATUS"),
                        propertyId = GetInt(reader, "PROPERTY_ID"),
                        propertyName = GetString(reader, "PROPERTY_NAME"),
                        buildingId = GetInt(reader, "BUILDING_ID"),
                        buildingName = GetString(reader, "BUILDING_NAME"),
                        floorId = GetInt(reader, "FLOOR_ID"),
                        floorName = GetString(reader, "FLOOR_NAME"),
                        roomId = GetInt(reader, "ROOM_ID"),
                        roomNo = GetString(reader, "ROOM_NO"),
                        roomName = GetString(reader, "ROOM_NAME"),
                        bedId = reader["BED_ID"] == DBNull.Value ? null : Convert.ToInt32(reader["BED_ID"]),
                        bedNo = reader["BED_NO"] == DBNull.Value ? null : Convert.ToString(reader["BED_NO"]),
                        guestName = GetString(reader, "GUEST_NAME"),
                        mobileNo = GetString(reader, "MOBILE_NO"),
                        emailId = GetNullableString(reader, "EMAIL_ID"),
                        occCatId = GetInt(reader, "OCC_CAT_ID"),
                        occCatName = GetString(reader, "OCC_CAT_NAME"),
                        patientName = GetNullableString(reader, "PATIENT_NAME"),
                        uhid = GetNullableString(reader, "UHID"),
                        wardName = GetNullableString(reader, "WARD_NAME"),
                        corporateName = GetNullableString(reader, "CORPORATE_NAME"),
                        checkinDate = Convert.ToDateTime(reader["CHECKIN_DT"]),
                        expectedCheckoutDate = Convert.ToDateTime(reader["EXPECTED_CHECKOUT_DT"]),
                        checkoutDate = Convert.ToDateTime(reader["CHECKOUT_DT"]),
                        totalNights = GetInt(reader, "TOTAL_NIGHTS"),
                        tariffId = GetInt(reader, "TARIFF_ID"),
                        tariffCode = GetString(reader, "TARIFF_CODE"),
                        tariffName = GetString(reader, "TARIFF_NAME"),
                        dailyRate = GetDecimal(reader, "DAILY_RATE"),
                        hourlyRate = GetDecimal(reader, "HOURLY_RATE"),
                        roomCharges = GetDecimal(reader, "ROOM_CHARGES"),
                        serviceCharges = GetDecimal(reader, "SERVICE_CHARGES"),
                        damageCharges = GetDecimal(reader, "DAMAGE_CHARGES"),
                        discountAmount = GetDecimal(reader, "DISCOUNT_AMOUNT"),
                        taxableAmount = GetDecimal(reader, "TAXABLE_AMOUNT"),
                        gstPercentage = GetDecimal(reader, "GST_PERCENTAGE"),
                        gstAmount = GetDecimal(reader, "GST_AMOUNT"),
                        roomGstAmount = GetDecimal(reader, "ROOM_GST_AMOUNT"),
                        serviceGstAmount = GetDecimal(reader, "SERVICE_GST_AMOUNT"),
                        grossAmount = GetDecimal(reader, "GROSS_AMOUNT"),
                        depositCollected = GetDecimal(reader, "DEPOSIT_COLLECTED"),
                        depositBalance = GetDecimal(reader, "DEPOSIT_BALANCE"),
                        depositAdjusted = GetDecimal(reader, "DEPOSIT_ADJUSTED"),
                        refundAmount = GetDecimal(reader, "REFUND_AMOUNT"),
                        netPayable = GetDecimal(reader, "NET_PAYABLE"),
                        amountPaid = GetDecimal(reader, "AMOUNT_PAID"),
                        balanceAmount = GetDecimal(reader, "BALANCE_AMOUNT"),
                        billId = GetInt(reader, "BILL_ID"),
                        billNo = GetString(reader, "BILL_NO"),
                        billStatus = GetString(reader, "BILL_STATUS")
                    };
                }

                //=========================================
                // Result Set - 2 (Service Charges)
                //=========================================

                if (details != null && reader.NextResult())
                {
                    while (reader.Read())
                    {
                        details.services.Add(new CheckoutServiceItem
                        {
                            serviceId = GetInt(reader, "SERVICE_ID"),
                            serviceCode = GetString(reader, "SERVICE_CODE"),
                            serviceName = GetString(reader, "SERVICE_NAME"),
                            quantity = GetDecimal(reader, "QTY"),
                            rate = GetDecimal(reader, "RATE"),
                            amount = GetDecimal(reader, "AMOUNT")
                        });
                    }
                }
            }

            //=========================================
            // NEW: laundry not yet delivered for this booking
            //=========================================

            if (details != null)
            {
                details.openLaundry = GetOpenLaundry(con, sessionId, bookingId);
            }

            return details;
        }

        private static List<CheckoutOpenLaundry> GetOpenLaundry(SqlConnection con, int sessionId, int bookingId)
        {
            List<CheckoutOpenLaundry> list = new();

            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_BOOKING_OPEN_LAUNDRY", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
            cmd.Parameters.AddWithValue("@IP_BOOKING_ID", bookingId);

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new CheckoutOpenLaundry
                {
                    laundryLogId = GetInt(reader, "LAUNDRY_LOG_ID"),
                    slipNo = GetString(reader, "LAUNDRY_SLIP_NO"),
                    laundryType = GetString(reader, "LAUNDRY_TYPE"),
                    itemCount = GetInt(reader, "ITEM_COUNT"),
                    status = GetString(reader, "STATUS"),
                    pickupDt = reader["PICKUP_DT"] == DBNull.Value ? null : Convert.ToDateTime(reader["PICKUP_DT"]),
                    createDt = Convert.ToDateTime(reader["CREATE_DT"])
                });
            }

            return list;
        }

        //=========================================================
        // CONFIRM CHECKOUT
        //=========================================================

        public CheckoutResult ConfirmCheckout(CheckoutRequest request)
        {
            CheckoutResult result = new CheckoutResult();

            using SqlConnection con = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_CHECKOUT", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IP_SESSION_ID", request.sessionId);
            cmd.Parameters.AddWithValue("@IP_BOOKING_ID", request.bookingId);
            cmd.Parameters.AddWithValue("@IP_CHECKOUT_DT", request.checkoutDt);

            // FIX: SP columns are CHAR(1) 'Y'/'N'. Passing a C# bool sent a
            // BIT, which SQL Server stored as '1'/'0'.
            cmd.Parameters.AddWithValue("@IP_KEY_RETURNED", request.keyReturned ? "Y" : "N");
            cmd.Parameters.AddWithValue("@IP_IS_COMPASSIONATE", request.isCompassionate ? "Y" : "N");

            cmd.Parameters.AddWithValue("@IP_DISCOUNT_AMT", request.discountAmount);
            cmd.Parameters.AddWithValue("@IP_DISCOUNT_REASON", string.IsNullOrWhiteSpace(request.discountReason) ? DBNull.Value : request.discountReason.Trim());
            cmd.Parameters.AddWithValue("@IP_PAYMENT_MODE", string.IsNullOrWhiteSpace(request.paymentMode) ? DBNull.Value : request.paymentMode);
            cmd.Parameters.AddWithValue("@IP_PAYMENT_AMOUNT", request.paymentAmount);
            cmd.Parameters.AddWithValue("@IP_PAYMENT_REF", string.IsNullOrWhiteSpace(request.paymentReference) ? DBNull.Value : request.paymentReference.Trim());
            cmd.Parameters.AddWithValue("@IP_PAYMENT_REMARKS", string.IsNullOrWhiteSpace(request.paymentRemarks) ? DBNull.Value : request.paymentRemarks);
            cmd.Parameters.AddWithValue("@IP_CARD_LAST4", string.IsNullOrWhiteSpace(request.cardLast4) ? DBNull.Value : request.cardLast4);
            cmd.Parameters.AddWithValue("@IP_UPI_VPA", string.IsNullOrWhiteSpace(request.upiVpa) ? DBNull.Value : request.upiVpa);

            cmd.Parameters.AddWithValue("@IP_REFUND_MODE", string.IsNullOrWhiteSpace(request.refundMode) ? DBNull.Value : request.refundMode);
            cmd.Parameters.AddWithValue("@IP_REFUND_REF", string.IsNullOrWhiteSpace(request.refundReference) ? DBNull.Value : request.refundReference);

            cmd.Parameters.AddWithValue("@IP_PENALTY_AMT", request.penaltyAmount);
            cmd.Parameters.AddWithValue("@IP_PENALTY_REASON_CD", string.IsNullOrWhiteSpace(request.penaltyReasonCode) ? DBNull.Value : request.penaltyReasonCode);
            cmd.Parameters.AddWithValue("@IP_PENALTY_REMARKS", string.IsNullOrWhiteSpace(request.penaltyRemarks) ? DBNull.Value : request.penaltyRemarks);

            // NEW: staff acknowledged laundry that is still not delivered
            cmd.Parameters.AddWithValue("@IP_LAUNDRY_ACK", request.laundryAcknowledged ? "Y" : "N");

            SqlParameter outBillId = new SqlParameter("@OUT_BILL_ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            SqlParameter outGross = new SqlParameter("@OUT_FINAL_GROSS", SqlDbType.Decimal) { Precision = 18, Scale = 2, Direction = ParameterDirection.Output };
            SqlParameter outRefund = new SqlParameter("@OUT_DEPOSIT_REFUND", SqlDbType.Decimal) { Precision = 18, Scale = 2, Direction = ParameterDirection.Output };
            SqlParameter outNet = new SqlParameter("@OUT_NET_PAYABLE", SqlDbType.Decimal) { Precision = 18, Scale = 2, Direction = ParameterDirection.Output };
            SqlParameter outPenalty = new SqlParameter("@OUT_PENALTY_AMT", SqlDbType.Decimal) { Precision = 18, Scale = 2, Direction = ParameterDirection.Output };
            SqlParameter outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
            SqlParameter outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };

            cmd.Parameters.Add(outBillId);
            cmd.Parameters.Add(outGross);
            cmd.Parameters.Add(outRefund);
            cmd.Parameters.Add(outNet);
            cmd.Parameters.Add(outPenalty);
            cmd.Parameters.Add(outStatus);
            cmd.Parameters.Add(outMessage);

            con.Open();
            cmd.ExecuteNonQuery();

            result.status = Convert.ToString(outStatus.Value) ?? "";
            result.message = Convert.ToString(outMessage.Value) ?? "";
            result.billId = outBillId.Value == DBNull.Value ? 0 : Convert.ToInt32(outBillId.Value);
            result.grossAmount = outGross.Value == DBNull.Value ? 0 : Convert.ToDecimal(outGross.Value);
            result.depositRefund = outRefund.Value == DBNull.Value ? 0 : Convert.ToDecimal(outRefund.Value);
            result.netPayable = outNet.Value == DBNull.Value ? 0 : Convert.ToDecimal(outNet.Value);
            result.penaltyAmount = outPenalty.Value == DBNull.Value ? 0 : Convert.ToDecimal(outPenalty.Value);
            result.bookingId = request.bookingId;
            result.checkoutDate = request.checkoutDt;
            result.paymentMode = request.paymentMode ?? "";
            result.paymentReceived = request.paymentAmount;

            return result;
        }

        //=========================================================
        // HELPERS — null-safe reads (a NULL in any numeric column used
        // to crash the whole screen with InvalidCastException)
        //=========================================================

        private static string GetString(SqlDataReader r, string col) =>
            r[col] == DBNull.Value ? "" : Convert.ToString(r[col]) ?? "";

        private static string? GetNullableString(SqlDataReader r, string col) =>
            r[col] == DBNull.Value ? null : Convert.ToString(r[col]);

        private static int GetInt(SqlDataReader r, string col) =>
            r[col] == DBNull.Value ? 0 : Convert.ToInt32(r[col]);

        private static decimal GetDecimal(SqlDataReader r, string col) =>
            r[col] == DBNull.Value ? 0 : Convert.ToDecimal(r[col]);
    }
}
