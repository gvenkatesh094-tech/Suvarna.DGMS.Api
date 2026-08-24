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
        // FIX: added optional search filters (booking no / guest name /
        // mobile / room no), matching the new SP signature.
        //=========================================================

        public List<CheckoutBooking> GetCheckoutBookings(
            int sessionId,
            string? bookingNo = null,
            string? guestName = null,
            string? mobileNo = null,
            string? roomNo = null)
        {
            List<CheckoutBooking> list = new List<CheckoutBooking>();

            using SqlConnection con = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_CHECKOUT_BOOKINGS", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
            cmd.Parameters.AddWithValue("@IP_BOOKING_NO", (object?)bookingNo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IP_GUEST_NAME", (object?)guestName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IP_MOBILE_NO", (object?)mobileNo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IP_ROOM_NO", (object?)roomNo ?? DBNull.Value);

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new CheckoutBooking
                {
                    bookingId = Convert.ToInt32(reader["BOOKING_ID"]),
                    bookingNo = Convert.ToString(reader["BOOKING_NO"]) ?? "",
                    guestName = Convert.ToString(reader["GUEST_NM"]) ?? "",
                    mobileNo = Convert.ToString(reader["MOBILE_NO"]) ?? "",
                    roomId = Convert.ToInt32(reader["ROOM_ID"]),
                    roomNo = Convert.ToString(reader["ROOM_NO"]) ?? "",
                    roomName = Convert.ToString(reader["ROOM_NM"]) ?? "",
                    checkinDate = Convert.ToDateTime(reader["ACTUAL_CHECKIN_DT"]),
                    expectedCheckoutDate = Convert.ToDateTime(reader["EXPECTED_CHECKOUT_DT"]),
                    stayDays = Convert.ToInt32(reader["STAY_DAYS"]),
                    totalBill = Convert.ToDecimal(reader["TOTAL_BILL"]),
                    deposit = Convert.ToDecimal(reader["DEPOSIT"]),
                    paidAmount = Convert.ToDecimal(reader["PAID_AMOUNT"]),
                    balanceAmount = Convert.ToDecimal(reader["BALANCE_AMOUNT"]),
                    bookingStatus = Convert.ToString(reader["BOOKING_STATUS"]) ?? "",
                    checkoutStatus = Convert.ToString(reader["CHECKOUT_STATUS"]) ?? ""
                });
            }

            return list;
        }

        //=========================================================
        // PREPARE / GET CHECKOUT DETAILS
        // FIX: PrepareCheckout no longer calls
        // GenerateAccommodationCharges() / CalculateBill() — those wrote
        // ACTUAL_CHECKOUT_DT and zeroed the deposit balance on EVERY
        // preview, permanently, even if the drawer was then cancelled.
        // Those two SPs are now DROPPED. PR_DGMS_GET_CHECKOUT_DETAILS is
        // 100% read-only and safe to call any number of times — so
        // "Prepare" now simply IS "Get Details".
        //=========================================================

        public CheckoutDetails? PrepareCheckout(int sessionId, int bookingId, DateTime checkoutDt)
        {
            return GetCheckoutDetails(sessionId, bookingId, checkoutDt);
        }

        public CheckoutDetails? GetCheckoutDetails(int sessionId, int bookingId, DateTime? checkoutDt)
        {
            CheckoutDetails? details = null;

            using SqlConnection con = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_CHECKOUT_DETAILS", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
            cmd.Parameters.AddWithValue("@IP_BOOKING_ID", bookingId);
            cmd.Parameters.AddWithValue("@IP_CHECKOUT_DT", (object?)checkoutDt ?? DBNull.Value);

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            //=========================================
            // Result Set - 1 (Header Details)
            //=========================================

            if (reader.Read())
            {
                details = new CheckoutDetails
                {
                    bookingId = Convert.ToInt32(reader["BOOKING_ID"]),
                    bookingNo = Convert.ToString(reader["BOOKING_NO"]) ?? "",
                    bookingStatus = Convert.ToString(reader["BOOKING_STATUS"]) ?? "",
                    propertyId = Convert.ToInt32(reader["PROPERTY_ID"]),
                    propertyName = Convert.ToString(reader["PROPERTY_NAME"]) ?? "",
                    buildingId = Convert.ToInt32(reader["BUILDING_ID"]),
                    buildingName = Convert.ToString(reader["BUILDING_NAME"]) ?? "",
                    floorId = Convert.ToInt32(reader["FLOOR_ID"]),
                    floorName = Convert.ToString(reader["FLOOR_NAME"]) ?? "",
                    roomId = Convert.ToInt32(reader["ROOM_ID"]),
                    roomNo = Convert.ToString(reader["ROOM_NO"]) ?? "",
                    roomName = Convert.ToString(reader["ROOM_NAME"]) ?? "",
                    bedId = reader["BED_ID"] == DBNull.Value ? null : Convert.ToInt32(reader["BED_ID"]),
                    bedNo = reader["BED_NO"] == DBNull.Value ? null : Convert.ToString(reader["BED_NO"]),
                    guestName = Convert.ToString(reader["GUEST_NAME"]) ?? "",
                    mobileNo = Convert.ToString(reader["MOBILE_NO"]) ?? "",
                    emailId = reader["EMAIL_ID"] == DBNull.Value ? null : Convert.ToString(reader["EMAIL_ID"]),
                    occCatId = Convert.ToInt32(reader["OCC_CAT_ID"]),
                    occCatName = Convert.ToString(reader["OCC_CAT_NAME"]) ?? "",
                    patientName = reader["PATIENT_NAME"] == DBNull.Value ? null : Convert.ToString(reader["PATIENT_NAME"]),
                    uhid = reader["UHID"] == DBNull.Value ? null : Convert.ToString(reader["UHID"]),
                    wardName = reader["WARD_NAME"] == DBNull.Value ? null : Convert.ToString(reader["WARD_NAME"]),
                    corporateName = reader["CORPORATE_NAME"] == DBNull.Value ? null : Convert.ToString(reader["CORPORATE_NAME"]),
                    checkinDate = Convert.ToDateTime(reader["CHECKIN_DT"]),
                    expectedCheckoutDate = Convert.ToDateTime(reader["EXPECTED_CHECKOUT_DT"]),
                    checkoutDate = Convert.ToDateTime(reader["CHECKOUT_DT"]),
                    totalNights = Convert.ToInt32(reader["TOTAL_NIGHTS"]),
                    tariffId = Convert.ToInt32(reader["TARIFF_ID"]),
                    tariffCode = Convert.ToString(reader["TARIFF_CODE"]) ?? "",
                    tariffName = Convert.ToString(reader["TARIFF_NAME"]) ?? "",
                    dailyRate = Convert.ToDecimal(reader["DAILY_RATE"]),
                    hourlyRate = Convert.ToDecimal(reader["HOURLY_RATE"]),
                    roomCharges = Convert.ToDecimal(reader["ROOM_CHARGES"]),
                    serviceCharges = Convert.ToDecimal(reader["SERVICE_CHARGES"]),
                    damageCharges = Convert.ToDecimal(reader["DAMAGE_CHARGES"]),
                    discountAmount = Convert.ToDecimal(reader["DISCOUNT_AMOUNT"]),
                    taxableAmount = Convert.ToDecimal(reader["TAXABLE_AMOUNT"]),
                    gstPercentage = Convert.ToDecimal(reader["GST_PERCENTAGE"]),
                    gstAmount = Convert.ToDecimal(reader["GST_AMOUNT"]),
                    grossAmount = Convert.ToDecimal(reader["GROSS_AMOUNT"]),
                    depositCollected = Convert.ToDecimal(reader["DEPOSIT_COLLECTED"]),
                    depositAdjusted = Convert.ToDecimal(reader["DEPOSIT_ADJUSTED"]),
                    refundAmount = Convert.ToDecimal(reader["REFUND_AMOUNT"]),
                    netPayable = Convert.ToDecimal(reader["NET_PAYABLE"]),
                    amountPaid = Convert.ToDecimal(reader["AMOUNT_PAID"]),
                    balanceAmount = Convert.ToDecimal(reader["BALANCE_AMOUNT"]),
                    billId = Convert.ToInt32(reader["BILL_ID"]),
                    billNo = Convert.ToString(reader["BILL_NO"]) ?? "",
                    billStatus = Convert.ToString(reader["BILL_STATUS"]) ?? ""
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
                        serviceId = Convert.ToInt32(reader["SERVICE_ID"]),
                        serviceCode = Convert.ToString(reader["SERVICE_CODE"]) ?? "",
                        serviceName = Convert.ToString(reader["SERVICE_NAME"]) ?? "",
                        quantity = Convert.ToDecimal(reader["QTY"]),
                        rate = Convert.ToDecimal(reader["RATE"]),
                        amount = Convert.ToDecimal(reader["AMOUNT"])
                    });
                }
            }

            return details;
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
            cmd.Parameters.AddWithValue("@IP_KEY_RETURNED", request.keyReturned);
            cmd.Parameters.AddWithValue("@IP_DISCOUNT_AMT", request.discountAmount);
            cmd.Parameters.AddWithValue("@IP_DISCOUNT_REASON", string.IsNullOrWhiteSpace(request.discountReason) ? DBNull.Value : request.discountReason);
            cmd.Parameters.AddWithValue("@IP_PAYMENT_MODE", string.IsNullOrWhiteSpace(request.paymentMode) ? DBNull.Value : request.paymentMode);
            cmd.Parameters.AddWithValue("@IP_PAYMENT_AMOUNT", request.paymentAmount);
            cmd.Parameters.AddWithValue("@IP_PAYMENT_REF", string.IsNullOrWhiteSpace(request.paymentReference) ? DBNull.Value : request.paymentReference);
            cmd.Parameters.AddWithValue("@IP_IS_COMPASSIONATE", request.isCompassionate);

            // NEW: deposit refund mode/reference (only meaningful when a
            // refund is actually due — SP handles null/empty gracefully).
            cmd.Parameters.AddWithValue("@IP_REFUND_MODE", string.IsNullOrWhiteSpace(request.refundMode) ? DBNull.Value : request.refundMode);
            cmd.Parameters.AddWithValue("@IP_REFUND_REF", string.IsNullOrWhiteSpace(request.refundReference) ? DBNull.Value : request.refundReference);

            SqlParameter outBillId = new SqlParameter("@OUT_BILL_ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outBillId);

            SqlParameter outGross = new SqlParameter("@OUT_FINAL_GROSS", SqlDbType.Decimal) { Precision = 18, Scale = 2, Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outGross);

            SqlParameter outRefund = new SqlParameter("@OUT_DEPOSIT_REFUND", SqlDbType.Decimal) { Precision = 18, Scale = 2, Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outRefund);

            SqlParameter outNet = new SqlParameter("@OUT_NET_PAYABLE", SqlDbType.Decimal) { Precision = 18, Scale = 2, Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outNet);

            SqlParameter outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outStatus);

            SqlParameter outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outMessage);

            con.Open();
            cmd.ExecuteNonQuery();

            result.status = Convert.ToString(outStatus.Value) ?? "";
            result.message = Convert.ToString(outMessage.Value) ?? "";
            result.billId = outBillId.Value == DBNull.Value ? 0 : Convert.ToInt32(outBillId.Value);
            result.grossAmount = outGross.Value == DBNull.Value ? 0 : Convert.ToDecimal(outGross.Value);
            result.depositRefund = outRefund.Value == DBNull.Value ? 0 : Convert.ToDecimal(outRefund.Value);
            result.netPayable = outNet.Value == DBNull.Value ? 0 : Convert.ToDecimal(outNet.Value);
            result.bookingId = request.bookingId;
            result.checkoutDate = request.checkoutDt;
            result.paymentMode = request.paymentMode;
            result.paymentReceived = request.paymentAmount;

            return result;
        }
    }
}
