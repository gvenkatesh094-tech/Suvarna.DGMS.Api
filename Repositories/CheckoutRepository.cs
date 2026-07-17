using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class CheckoutRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public CheckoutRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString =
                _configuration.GetConnectionString("DefaultConnection")!;
        }

        //=========================================================
        // GET CHECKOUT BOOKINGS
        //=========================================================

        public List<CheckoutBooking> GetCheckoutBookings(
            string sessionId,
            int? propertyId,
            string? bookingNo,
            string? guestName,
            string? mobileNo,
            string? roomNo)
        {
            List<CheckoutBooking> list =
                new List<CheckoutBooking>();

            using SqlConnection con =
                new SqlConnection(_connectionString);

            using SqlCommand cmd =
                new SqlCommand(
                    "DGMS.PR_DGMS_GET_CHECKOUT_BOOKINGS",
                    con);

            cmd.CommandType =
                CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue(
                "@IP_SESSION_ID",
                sessionId);

            cmd.Parameters.AddWithValue(
                "@IP_PROPERTY_ID",
                propertyId.HasValue
                    ? propertyId.Value
                    : DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@IP_BOOKING_NO",
                string.IsNullOrWhiteSpace(bookingNo)
                    ? DBNull.Value
                    : bookingNo);

            cmd.Parameters.AddWithValue(
                "@IP_GUEST_NAME",
                string.IsNullOrWhiteSpace(guestName)
                    ? DBNull.Value
                    : guestName);

            cmd.Parameters.AddWithValue(
                "@IP_MOBILE_NO",
                string.IsNullOrWhiteSpace(mobileNo)
                    ? DBNull.Value
                    : mobileNo);

            cmd.Parameters.AddWithValue(
                "@IP_ROOM_NO",
                string.IsNullOrWhiteSpace(roomNo)
                    ? DBNull.Value
                    : roomNo);

            con.Open();

            using SqlDataReader reader =
                cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(
                    new CheckoutBooking
                    {
                        bookingId =
                            Convert.ToInt32(
                                reader["BOOKING_ID"]),

                        bookingNo =
                            Convert.ToString(
                                reader["BOOKING_NO"]) ?? "",

                        guestName =
                            Convert.ToString(
                                reader["GUEST_NAME"]) ?? "",

                        mobileNo =
                            Convert.ToString(
                                reader["MOBILE_NO"]) ?? "",

                        propertyId =
                            Convert.ToInt32(
                                reader["PROPERTY_ID"]),

                        propertyName =
                            Convert.ToString(
                                reader["PROPERTY_NAME"]) ?? "",

                        roomId =
                            Convert.ToInt32(
                                reader["ROOM_ID"]),

                        roomNo =
                            Convert.ToString(
                                reader["ROOM_NO"]) ?? "",

                        bedId =
                            reader["BED_ID"] == DBNull.Value
                                ? null
                                : Convert.ToInt32(
                                    reader["BED_ID"]),

                        bedNo =
                            reader["BED_NO"] == DBNull.Value
                                ? null
                                : Convert.ToString(
                                    reader["BED_NO"]),

                        tariffId =
                            Convert.ToInt32(
                                reader["TARIFF_ID"]),

                        tariffName =
                            Convert.ToString(
                                reader["TARIFF_NAME"]) ?? "",

                        occCatId =
                            Convert.ToInt32(
                                reader["OCC_CAT_ID"]),

                        occCatName =
                            Convert.ToString(
                                reader["OCC_CAT_NAME"]) ?? "",

                        checkinDate =
                            Convert.ToDateTime(
                                reader["CHECKIN_DT"]),

                        expectedCheckoutDate =
                            Convert.ToDateTime(
                                reader["EXPECTED_CHECKOUT_DT"]),

                        totalNights =
                            Convert.ToInt32(
                                reader["TOTAL_NIGHTS"]),

                        roomCharges =
                            Convert.ToDecimal(
                                reader["ROOM_CHARGES"]),

                        serviceCharges =
                            Convert.ToDecimal(
                                reader["SERVICE_CHARGES"]),

                        grossAmount =
                            Convert.ToDecimal(
                                reader["GROSS_AMOUNT"]),

                        depositAmount =
                            Convert.ToDecimal(
                                reader["DEPOSIT_AMOUNT"]),

                        balanceAmount =
                            Convert.ToDecimal(
                                reader["BALANCE_AMOUNT"]),

                        bookingStatus =
                            Convert.ToString(
                                reader["BOOKING_STATUS"]) ?? "",

                        patientName =
                            reader["PATIENT_NAME"] == DBNull.Value
                                ? null
                                : Convert.ToString(
                                    reader["PATIENT_NAME"]),

                        uhid =
                            reader["UHID"] == DBNull.Value
                                ? null
                                : Convert.ToString(
                                    reader["UHID"]),

                        wardName =
                            reader["WARD_NAME"] == DBNull.Value
                                ? null
                                : Convert.ToString(
                                    reader["WARD_NAME"]),

                        corporateName =
                            reader["CORPORATE_NAME"] == DBNull.Value
                                ? null
                                : Convert.ToString(
                                    reader["CORPORATE_NAME"])
                    });
            }

            return list;
        }
        //=========================================================
        // GET CHECKOUT DETAILS
        //=========================================================

        public CheckoutDetails? GetCheckoutDetails(
            string sessionId,
            int bookingId)
        {
            CheckoutDetails? details = null;

            using SqlConnection con =
                new SqlConnection(_connectionString);

            using SqlCommand cmd =
                new SqlCommand(
                    "DGMS.PR_DGMS_GET_CHECKOUT_DETAILS",
                    con);

            cmd.CommandType =
                CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue(
                "@IP_SESSION_ID",
                sessionId);

            cmd.Parameters.AddWithValue(
                "@IP_BOOKING_ID",
                bookingId);

            con.Open();

            using SqlDataReader reader =
                cmd.ExecuteReader();

            //=========================================
            // Result Set - 1 (Header Details)
            //=========================================

            if (reader.Read())
            {
                details = new CheckoutDetails
                {
                    bookingId =
                        Convert.ToInt32(reader["BOOKING_ID"]),

                    bookingNo =
                        Convert.ToString(reader["BOOKING_NO"]) ?? "",

                    bookingStatus =
                        Convert.ToString(reader["BOOKING_STATUS"]) ?? "",

                    propertyId =
                        Convert.ToInt32(reader["PROPERTY_ID"]),

                    propertyName =
                        Convert.ToString(reader["PROPERTY_NAME"]) ?? "",

                    buildingId =
                        Convert.ToInt32(reader["BUILDING_ID"]),

                    buildingName =
                        Convert.ToString(reader["BUILDING_NAME"]) ?? "",

                    floorId =
                        Convert.ToInt32(reader["FLOOR_ID"]),

                    floorName =
                        Convert.ToString(reader["FLOOR_NAME"]) ?? "",

                    roomId =
                        Convert.ToInt32(reader["ROOM_ID"]),

                    roomNo =
                        Convert.ToString(reader["ROOM_NO"]) ?? "",

                    bedId =
                        reader["BED_ID"] == DBNull.Value
                            ? null
                            : Convert.ToInt32(reader["BED_ID"]),

                    bedNo =
                        reader["BED_NO"] == DBNull.Value
                            ? null
                            : Convert.ToString(reader["BED_NO"]),

                    guestName =
                        Convert.ToString(reader["GUEST_NAME"]) ?? "",

                    mobileNo =
                        Convert.ToString(reader["MOBILE_NO"]) ?? "",

                    emailId =
                        reader["EMAIL_ID"] == DBNull.Value
                            ? null
                            : Convert.ToString(reader["EMAIL_ID"]),

                    occCatId =
                        Convert.ToInt32(reader["OCC_CAT_ID"]),

                    occCatName =
                        Convert.ToString(reader["OCC_CAT_NAME"]) ?? "",

                    patientName =
                        reader["PATIENT_NAME"] == DBNull.Value
                            ? null
                            : Convert.ToString(reader["PATIENT_NAME"]),

                    uhid =
                        reader["UHID"] == DBNull.Value
                            ? null
                            : Convert.ToString(reader["UHID"]),

                    wardName =
                        reader["WARD_NAME"] == DBNull.Value
                            ? null
                            : Convert.ToString(reader["WARD_NAME"]),

                    corporateName =
                        reader["CORPORATE_NAME"] == DBNull.Value
                            ? null
                            : Convert.ToString(reader["CORPORATE_NAME"]),

                    checkinDate =
                        Convert.ToDateTime(reader["CHECKIN_DT"]),

                    expectedCheckoutDate =
                        Convert.ToDateTime(reader["EXPECTED_CHECKOUT_DT"]),

                    checkoutDate =
                        Convert.ToDateTime(reader["CHECKOUT_DT"]),

                    totalNights =
                        Convert.ToInt32(reader["TOTAL_NIGHTS"]),

                    tariffId =
                        Convert.ToInt32(reader["TARIFF_ID"]),

                    tariffCode =
                        Convert.ToString(reader["TARIFF_CODE"]) ?? "",

                    tariffName =
                        Convert.ToString(reader["TARIFF_NAME"]) ?? "",

                    dailyRate =
                        Convert.ToDecimal(reader["DAILY_RATE"]),

                    hourlyRate =
                        Convert.ToDecimal(reader["HOURLY_RATE"]),

                    roomCharges =
                        Convert.ToDecimal(reader["ROOM_CHARGES"]),

                    serviceCharges =
                        Convert.ToDecimal(reader["SERVICE_CHARGES"]),

                    damageCharges =
                        Convert.ToDecimal(reader["DAMAGE_CHARGES"]),

                    discountAmount =
                        Convert.ToDecimal(reader["DISCOUNT_AMOUNT"]),

                    taxableAmount =
                        Convert.ToDecimal(reader["TAXABLE_AMOUNT"]),

                    gstPercentage =
                        Convert.ToDecimal(reader["GST_PERCENTAGE"]),

                    gstAmount =
                        Convert.ToDecimal(reader["GST_AMOUNT"]),

                    grossAmount =
                        Convert.ToDecimal(reader["GROSS_AMOUNT"]),

                    depositCollected =
                        Convert.ToDecimal(reader["DEPOSIT_COLLECTED"]),

                    depositAdjusted =
                        Convert.ToDecimal(reader["DEPOSIT_ADJUSTED"]),

                    refundAmount =
                        Convert.ToDecimal(reader["REFUND_AMOUNT"]),

                    netPayable =
                        Convert.ToDecimal(reader["NET_PAYABLE"]),

                    amountPaid =
                        Convert.ToDecimal(reader["AMOUNT_PAID"]),

                    balanceAmount =
                        Convert.ToDecimal(reader["BALANCE_AMOUNT"]),

                    billId =
                        Convert.ToInt32(reader["BILL_ID"]),

                    billNo =
                        Convert.ToString(reader["BILL_NO"]) ?? "",

                    billStatus =
                        Convert.ToString(reader["BILL_STATUS"]) ?? ""
                };
            }

            //=========================================
            // Result Set - 2 (Service Charges)
            //=========================================

            if (details != null && reader.NextResult())
            {
                while (reader.Read())
                {
                    details.services.Add(
                        new CheckoutServiceItem
                        {
                            serviceId =
                                Convert.ToInt32(reader["SERVICE_ID"]),

                            serviceCode =
                                Convert.ToString(reader["SERVICE_CODE"]) ?? "",

                            serviceName =
                                Convert.ToString(reader["SERVICE_NAME"]) ?? "",

                            quantity =
                                Convert.ToDecimal(reader["QTY"]),

                            rate =
                                Convert.ToDecimal(reader["RATE"]),

                            amount =
                                Convert.ToDecimal(reader["AMOUNT"])
                        });
                }
            }

            return details;
        }
        //=========================================================
        // CONFIRM CHECKOUT
        //=========================================================

        public CheckoutResult ConfirmCheckout(
            CheckoutRequest request)
        {
            CheckoutResult result =
                new CheckoutResult();

            using SqlConnection con =
                new SqlConnection(_connectionString);

            using SqlCommand cmd =
                new SqlCommand(
                    "DGMS.PR_DGMS_CHECKOUT",
                    con);

            cmd.CommandType =
                CommandType.StoredProcedure;

            //=========================================
            // INPUT PARAMETERS
            //=========================================

            cmd.Parameters.AddWithValue(
                "@IP_SESSION_ID",
                request.sessionId);

            cmd.Parameters.AddWithValue(
                "@IP_BOOKING_ID",
                request.bookingId);

            cmd.Parameters.AddWithValue(
                "@IP_CHECKOUT_DT",
                request.checkoutDt);

            cmd.Parameters.AddWithValue(
                "@IP_PAYMENT_MODE",
                request.paymentMode);

            cmd.Parameters.AddWithValue(
                "@IP_PAYMENT_AMOUNT",
                request.paymentAmount);

            cmd.Parameters.AddWithValue(
                "@IP_PAYMENT_REFERENCE",
                string.IsNullOrWhiteSpace(request.paymentReference)
                    ? DBNull.Value
                    : request.paymentReference);

            cmd.Parameters.AddWithValue(
                "@IP_DISCOUNT_AMOUNT",
                request.discountAmount);

            cmd.Parameters.AddWithValue(
                "@IP_DISCOUNT_REASON",
                string.IsNullOrWhiteSpace(request.discountReason)
                    ? DBNull.Value
                    : request.discountReason);

            cmd.Parameters.AddWithValue(
                "@IP_ROOM_INSPECTED",
                request.roomInspected);

            cmd.Parameters.AddWithValue(
                "@IP_KEY_RETURNED",
                request.keyReturned);

            cmd.Parameters.AddWithValue(
                "@IP_SIGNATURE_OBTAINED",
                request.signatureObtained);

            cmd.Parameters.AddWithValue(
                "@IP_IS_COMPASSIONATE",
                request.isCompassionate);

            cmd.Parameters.AddWithValue(
                "@IP_REMARKS",
                string.IsNullOrWhiteSpace(request.remarks)
                    ? DBNull.Value
                    : request.remarks);

            //=========================================
            // EXECUTE
            //=========================================

            con.Open();

            using SqlDataReader reader =
                cmd.ExecuteReader();

            if (reader.Read())
            {
                result.status =
                    Convert.ToString(
                        reader["STATUS"]) ?? "";

                result.message =
                    Convert.ToString(
                        reader["MESSAGE"]) ?? "";

                result.bookingId =
                    Convert.ToInt32(
                        reader["BOOKING_ID"]);

                result.bookingNo =
                    Convert.ToString(
                        reader["BOOKING_NO"]) ?? "";

                result.checkoutDate =
                    Convert.ToDateTime(
                        reader["CHECKOUT_DT"]);

                result.billId =
                    Convert.ToInt32(
                        reader["BILL_ID"]);

                result.billNo =
                    Convert.ToString(
                        reader["BILL_NO"]) ?? "";

                result.grossAmount =
                    Convert.ToDecimal(
                        reader["GROSS_AMOUNT"]);

                result.discountAmount =
                    Convert.ToDecimal(
                        reader["DISCOUNT_AMOUNT"]);

                result.gstAmount =
                    Convert.ToDecimal(
                        reader["GST_AMOUNT"]);

                result.netPayable =
                    Convert.ToDecimal(
                        reader["NET_PAYABLE"]);

                result.depositAdjusted =
                    Convert.ToDecimal(
                        reader["DEPOSIT_ADJUSTED"]);

                result.depositRefund =
                    Convert.ToDecimal(
                        reader["DEPOSIT_REFUND"]);

                result.paymentReceived =
                    Convert.ToDecimal(
                        reader["PAYMENT_RECEIVED"]);

                result.paymentMode =
                    Convert.ToString(
                        reader["PAYMENT_MODE"]) ?? "";

                result.paymentReference =
                    reader["PAYMENT_REFERENCE"] == DBNull.Value
                        ? null
                        : Convert.ToString(
                            reader["PAYMENT_REFERENCE"]);

                result.paymentTxnId =
                    Convert.ToInt32(
                        reader["PAYMENT_TXN_ID"]);

                result.receiptNo =
                    Convert.ToString(
                        reader["RECEIPT_NO"]) ?? "";

                result.roomId =
                    Convert.ToInt32(
                        reader["ROOM_ID"]);

                result.roomNo =
                    Convert.ToString(
                        reader["ROOM_NO"]) ?? "";

                result.bedId =
                    reader["BED_ID"] == DBNull.Value
                        ? null
                        : Convert.ToInt32(
                            reader["BED_ID"]);

                result.bedNo =
                    reader["BED_NO"] == DBNull.Value
                        ? null
                        : Convert.ToString(
                            reader["BED_NO"]);

                result.roomReleased =
                    Convert.ToBoolean(
                        reader["ROOM_RELEASED"]);

                result.bedReleased =
                    Convert.ToBoolean(
                        reader["BED_RELEASED"]);

                result.checkoutCompleted =
                    Convert.ToBoolean(
                        reader["CHECKOUT_COMPLETED"]);
            }

            return result;
        }
        private CheckoutBooking MapCheckoutBooking(SqlDataReader reader)
        {
            return new CheckoutBooking
            {
                bookingId = Convert.ToInt32(reader["BOOKING_ID"]),
                bookingNo = Convert.ToString(reader["BOOKING_NO"]) ?? "",
                guestName = Convert.ToString(reader["GUEST_NAME"]) ?? "",
                mobileNo = Convert.ToString(reader["MOBILE_NO"]) ?? "",

                propertyId = Convert.ToInt32(reader["PROPERTY_ID"]),
                propertyName = Convert.ToString(reader["PROPERTY_NAME"]) ?? "",

                roomId = Convert.ToInt32(reader["ROOM_ID"]),
                roomNo = Convert.ToString(reader["ROOM_NO"]) ?? "",

                bedId = reader["BED_ID"] == DBNull.Value
                            ? null
                            : Convert.ToInt32(reader["BED_ID"]),

                bedNo = reader["BED_NO"] == DBNull.Value
                            ? null
                            : Convert.ToString(reader["BED_NO"]),

                tariffId = Convert.ToInt32(reader["TARIFF_ID"]),
                tariffName = Convert.ToString(reader["TARIFF_NAME"]) ?? "",

                occCatId = Convert.ToInt32(reader["OCC_CAT_ID"]),
                occCatName = Convert.ToString(reader["OCC_CAT_NAME"]) ?? "",

                checkinDate = Convert.ToDateTime(reader["CHECKIN_DT"]),
                expectedCheckoutDate = Convert.ToDateTime(reader["EXPECTED_CHECKOUT_DT"]),

                totalNights = Convert.ToInt32(reader["TOTAL_NIGHTS"]),

                roomCharges = Convert.ToDecimal(reader["ROOM_CHARGES"]),
                serviceCharges = Convert.ToDecimal(reader["SERVICE_CHARGES"]),
                grossAmount = Convert.ToDecimal(reader["GROSS_AMOUNT"]),
                depositAmount = Convert.ToDecimal(reader["DEPOSIT_AMOUNT"]),
                balanceAmount = Convert.ToDecimal(reader["BALANCE_AMOUNT"]),

                bookingStatus = Convert.ToString(reader["BOOKING_STATUS"]) ?? "",

                patientName = reader["PATIENT_NAME"] == DBNull.Value
                                    ? null
                                    : Convert.ToString(reader["PATIENT_NAME"]),

                uhid = reader["UHID"] == DBNull.Value
                            ? null
                            : Convert.ToString(reader["UHID"]),

                wardName = reader["WARD_NAME"] == DBNull.Value
                            ? null
                            : Convert.ToString(reader["WARD_NAME"]),

                corporateName = reader["CORPORATE_NAME"] == DBNull.Value
                            ? null
                            : Convert.ToString(reader["CORPORATE_NAME"])
            };
        }
        private CheckoutDetails MapCheckoutDetails(    SqlDataReader reader)
        {
            return new CheckoutDetails
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

                bedId = reader["BED_ID"] == DBNull.Value
                            ? null
                            : Convert.ToInt32(reader["BED_ID"]),

                bedNo = reader["BED_NO"] == DBNull.Value
                            ? null
                            : Convert.ToString(reader["BED_NO"]),

                guestName = Convert.ToString(reader["GUEST_NAME"]) ?? "",
                mobileNo = Convert.ToString(reader["MOBILE_NO"]) ?? "",

                emailId = reader["EMAIL_ID"] == DBNull.Value
                            ? null
                            : Convert.ToString(reader["EMAIL_ID"]),

                occCatId = Convert.ToInt32(reader["OCC_CAT_ID"]),
                occCatName = Convert.ToString(reader["OCC_CAT_NAME"]) ?? "",

                patientName = reader["PATIENT_NAME"] == DBNull.Value
                                    ? null
                                    : Convert.ToString(reader["PATIENT_NAME"]),

                uhid = reader["UHID"] == DBNull.Value
                            ? null
                            : Convert.ToString(reader["UHID"]),

                wardName = reader["WARD_NAME"] == DBNull.Value
                            ? null
                            : Convert.ToString(reader["WARD_NAME"]),

                corporateName = reader["CORPORATE_NAME"] == DBNull.Value
                            ? null
                            : Convert.ToString(reader["CORPORATE_NAME"]),

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
        private CheckoutServiceItem MapCheckoutService(SqlDataReader reader)
        {
            return new CheckoutServiceItem
            {
                serviceId = Convert.ToInt32(reader["SERVICE_ID"]),
                serviceCode = Convert.ToString(reader["SERVICE_CODE"]) ?? "",
                serviceName = Convert.ToString(reader["SERVICE_NAME"]) ?? "",
                quantity = Convert.ToDecimal(reader["QTY"]),
                rate = Convert.ToDecimal(reader["RATE"]),
                amount = Convert.ToDecimal(reader["AMOUNT"])
            };
        }
    }
}
