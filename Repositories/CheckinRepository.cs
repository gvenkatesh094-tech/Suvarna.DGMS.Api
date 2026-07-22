using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class CheckinRepository
    {
        private readonly string _conn;

        public CheckinRepository(
            IConfiguration configuration)
        {
            _conn = configuration
                .GetConnectionString("SqlServer");
        }

        // ==========================================
        // GET AVAILABLE ROOMS
        // ==========================================

        public List<CheckinRoom> GetRooms(
            string sessionId,
            int? propertyId,
            int? buildingId,
            int? floorId,
            int? accmTypeId,
            string? roomStatus,
            string? isAc,
            string? isAccessible)
        {
            List<CheckinRoom> list =
                new List<CheckinRoom>();

            try
            {
                using SqlConnection conn =
                    new SqlConnection(_conn);

                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_ROOM_CHECKIN_LOOKUP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_PROPERTY_ID", propertyId.HasValue ? propertyId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@IP_ROOM_STATUS_CD", (object?)roomStatus ?? DBNull.Value);
                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(
                        new CheckinRoom
                        {
                            roomId = GetInt(dr, "ROOM_ID"),
                            roomNo = GetString(dr, "ROOM_NO"),
                            roomName = GetString(dr, "ROOM_NM"),
                            roomStatus = GetString(dr, "ROOM_STATUS_CD"),
                            propertyId = GetInt(dr, "PROPERTY_ID"),
                            propertyName = GetString(dr, "PROPERTY_NM"),
                            propertyCode = GetString(dr, "PROPERTY_CD"),
                            buildingId = GetNullableInt(dr, "BUILDING_ID"),
                            buildingName = GetString(dr, "BUILDING_NM"),
                            floorId = GetNullableInt(dr, "FLOOR_ID"),
                            floorName = GetString(dr, "FLOOR_NM"),
                            floorSeq = GetInt(dr, "FLOOR_SEQ"),
                            accmTypeId = GetInt(dr, "ACCM_TYPE_ID"),
                            accmTypeName = GetString(dr, "ACCM_TYPE_NM"),
                            accmCategory = GetString(dr, "ACCM_CATEGORY"),
                            maxOccupants = GetInt(dr, "MAX_OCCUPANTS"),
                            totalBeds = GetInt(dr, "TOTAL_BEDS"),
                            availableBeds = GetInt(dr, "AVAILABLE_BEDS"),
                            isAc = GetString(dr, "IS_AC"),
                            isAttachedBath = GetString(dr, "IS_ATTACHED_BATH"),
                            isWifi = GetString(dr, "IS_WIFI"),
                            isTv = GetString(dr, "IS_TV"),
                            isAccessible = GetString(dr, "IS_ACCESSIBLE"),
                            wardProximity = GetString(dr, "WARD_PROXIMITY"),
                            blockedReason = GetString(dr, "BLOCKED_REASON"),
                            blockedFromDt = GetNullableDateTime(dr, "BLOCKED_FROM_DT"),
                            blockedToDt = GetNullableDateTime(dr, "BLOCKED_TO_DT")
                        }
                    );
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Error while fetching available rooms.",
                    ex
                );
            }
        }

        // ==========================================
        // GET TARIFF
        // ==========================================

        public List<CheckinTariff> GetTariff(
     string sessionId,
     int propertyId,
     int accmTypeId,
     int occCatId,
     DateTime checkinDt,
     string? billingModel)
        {
            try
            {
                using SqlConnection conn =
                    new SqlConnection(_conn);

                using SqlCommand cmd =
                    new SqlCommand(
                        "DGMS.PR_DGMS_GET_TARIFF",
                        conn
                    );

                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@IP_SESSION_ID",
                    sessionId
                );

                cmd.Parameters.AddWithValue(
                    "@IP_PROPERTY_ID",
                    propertyId
                );

                cmd.Parameters.AddWithValue(
                    "@IP_ACCM_TYPE_ID",
                    accmTypeId
                );

                cmd.Parameters.AddWithValue(
                    "@IP_OCC_CAT_ID",
                    occCatId
                );

                cmd.Parameters.AddWithValue(
                    "@IP_CHECKIN_DT",
                    checkinDt
                );

                cmd.Parameters.AddWithValue(
                    "@IP_BILLING_MODEL",
                    (object?)billingModel
                    ?? DBNull.Value
                );

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();

                List<CheckinTariff> tariffs = new();

                while (dr.Read())
                {
                    tariffs.Add(new CheckinTariff
                    {
                        tariffId = GetInt(dr, "TARIFF_ID"),

                        tariffCode = GetString(dr, "TARIFF_CD"),

                        tariffName = GetString(dr, "TARIFF_NM"),

                        tariffType = GetString(dr, "TARIFF_TYPE_CD"),

                        billingModel = GetString(dr, "BILLING_MODEL_CD"),

                        dailyRate = GetDecimal(dr, "DAILY_RATE_AMT"),

                        hourlyRate = GetDecimal(dr, "HOURLY_RATE_AMT"),

                        monthlyRate = GetDecimal(dr, "MONTHLY_RATE_AMT"),

                        packageRate = GetDecimal(dr, "PACKAGE_RATE_AMT"),

                        packageDays = GetInt(dr, "PACKAGE_DAYS"),

                        minBillingHours = GetInt(dr, "MIN_BILLING_HOURS"),

                        checkinHour = GetInt(dr, "CHECKIN_HOUR"),

                        checkoutHour = GetInt(dr, "CHECKOUT_HOUR"),

                        gstSlabPct = GetDecimal(dr, "GST_SLAB_PCT"),

                        isGstInclusive = GetString(dr, "IS_GST_INCLUSIVE"),

                        sacCode = GetString(dr, "SAC_CODE"),

                        cgstPct = GetDecimal(dr, "CGST_PCT"),

                        sgstPct = GetDecimal(dr, "SGST_PCT"),

                        igstPct = GetDecimal(dr, "IGST_PCT"),

                        isGstExempt = GetString(dr, "IS_GST_EXEMPT"),

                        securityDepositAmount = GetDecimal(dr, "SECURITY_DEPOSIT_AMT"),

                        isDepositRefundable = GetString(dr, "IS_DEPOSIT_REFUNDABLE"),

                        baseDailyAmount = GetDecimal(dr, "BASE_DAILY_AMT"),

                        gstAmountPerDay = GetDecimal(dr, "GST_AMT_PER_DAY")
                    });
                }

                return tariffs;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Error while fetching tariff.",
                    ex
                );
            }
        }

        // ==========================================
        // CONFIRM CHECK-IN
        // ==========================================

        public CheckinResult ConfirmCheckin(
            Checkin request)
        {
            try
            {
                using SqlConnection conn =
                    new SqlConnection(_conn);

                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_CHECKIN", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                AddParameter(cmd, "@IP_SESSION_ID", request.sessionId);
                AddParameter(cmd, "@IP_EXISTING_BOOKING_ID", request.existingBookingId);
                AddParameter(cmd, "@IP_PROPERTY_ID", request.propertyId);
                AddParameter(cmd, "@IP_ROOM_ID", request.roomId);
                AddParameter(cmd, "@IP_BED_ID", request.bedId);
                AddParameter(cmd, "@IP_TARIFF_ID", request.tariffId);
                AddParameter(cmd, "@IP_OCC_CAT_ID", request.occCatId);
                AddParameter(cmd, "@IP_GUEST_NM", request.guestName?.Trim());
                AddParameter(cmd, "@IP_GENDER_CD", request.genderCd);
                AddParameter(cmd, "@IP_DOB", request.dob);
                AddParameter(cmd, "@IP_MOBILE_NO", request.mobileNo?.Trim());
                AddParameter(cmd, "@IP_EMAIL_ID", request.emailId?.Trim());
                AddParameter(cmd, "@IP_ID_TYPE_CD", request.idTypeCd);
                AddParameter(cmd, "@IP_ID_NUMBER_MASKED", request.idNumberMasked?.Trim());
                AddParameter(cmd, "@IP_PHOTO_PATH", request.photoPath);
                AddParameter(cmd, "@IP_GUEST_GSTIN", request.guestGstin);
                AddParameter(cmd, "@IP_GUEST_STATE_CD", request.guestStateCd);
                AddParameter(cmd, "@IP_VEHICLE_NO", request.vehicleNo);
                AddParameter(cmd, "@IP_PURPOSE", request.purpose);
                AddParameter(cmd, "@IP_ADDRESS_LINE1", request.addressLine1);
                AddParameter(cmd, "@IP_CITY", request.city);
                AddParameter(cmd, "@IP_STATE_CD", request.stateCd);
                AddParameter(cmd, "@IP_EMERGENCY_NM", request.emergencyName);
                AddParameter(cmd, "@IP_EMERGENCY_PH", request.emergencyPhone);
                AddParameter(cmd, "@IP_PATIENT_ID", request.patientId);
                AddParameter(cmd, "@IP_UMR_NO", request.umrNo);
                AddParameter(cmd, "@IP_ADMN_ID", request.admnId);
                AddParameter(cmd, "@IP_ADMN_NO", request.admnNo);
                AddParameter(cmd, "@IP_RELATIONSHIP_CD", request.relationshipCd);
                AddParameter(cmd, "@IP_CORP_AGREEMENT_ID", request.corpAgreementId);
                AddParameter(cmd, "@IP_CORP_PO_REF", request.corpPoRef);
                AddParameter(cmd, "@IP_EXPECTED_CHECKOUT_DT", request.expectedCheckoutDt);
                AddParameter(cmd, "@IP_DEPOSIT_AMT", request.depositAmount);
                AddParameter(cmd, "@IP_DEPOSIT_PAYMENT_MODE", request.depositPaymentMode);
                AddParameter(cmd, "@IP_DEPOSIT_REF", request.depositRef);

                SqlParameter bookingId = new SqlParameter("@OUT_BOOKING_ID", SqlDbType.Int);
                bookingId.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(bookingId);

                SqlParameter bookingNo = new SqlParameter("@OUT_BOOKING_NO", SqlDbType.VarChar, 30);
                bookingNo.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(bookingNo);

                SqlParameter billId = new SqlParameter("@OUT_BILL_ID", SqlDbType.Int);

                billId.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(billId);

                SqlParameter billNo = new SqlParameter("@OUT_BILL_NO", SqlDbType.VarChar, 30);

                billNo.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(billNo);

                SqlParameter status = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1);

                status.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(status);

                SqlParameter message = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500);

                message.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(message);

                conn.Open();

                cmd.ExecuteNonQuery();

                return new CheckinResult
                {
                    bookingId =
                        bookingId.Value == DBNull.Value
                            ? 0
                            : Convert.ToInt32(
                                bookingId.Value
                            ),

                    bookingNo =
                        Convert.ToString(
                            bookingNo.Value
                        ) ?? "",

                    billId =
                        billId.Value == DBNull.Value
                            ? 0
                            : Convert.ToInt32(
                                billId.Value
                            ),

                    billNo =
                        Convert.ToString(
                            billNo.Value
                        ) ?? "",

                    status =
                        Convert.ToString(
                            status.Value
                        ) ?? "",

                    message =
                        Convert.ToString(
                            message.Value
                        ) ?? ""
                };
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Error while confirming check-in.",
                    ex
                );
            }
        }

        // ==========================================
        // COMMON HELPERS
        // ==========================================

        private static void AddParameter(
            SqlCommand cmd,
            string name,
            object? value)
        {
            cmd.Parameters.AddWithValue(
                name,
                value ?? DBNull.Value
            );
        }

        private static string? GetString(
            SqlDataReader dr,
            string column)
        {
            return dr[column] == DBNull.Value
                ? null
                : Convert.ToString(dr[column]);
        }

        private static int GetInt(
            SqlDataReader dr,
            string column)
        {
            return dr[column] == DBNull.Value
                ? 0
                : Convert.ToInt32(dr[column]);
        }

        private static int? GetNullableInt(
            SqlDataReader dr,
            string column)
        {
            return dr[column] == DBNull.Value
                ? null
                : Convert.ToInt32(dr[column]);
        }

        private static decimal GetDecimal(
            SqlDataReader dr,
            string column)
        {
            return dr[column] == DBNull.Value
                ? 0
                : Convert.ToDecimal(dr[column]);
        }

        private static DateTime? GetNullableDateTime(
            SqlDataReader dr,
            string column)
        {
            return dr[column] == DBNull.Value
                ? null
                : Convert.ToDateTime(dr[column]);
        }
    }
}