using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Helpers;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class ReservationsRepository
    {
        private readonly string _connectionString;

        public ReservationsRepository(
            IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString(
                    "SqlServer"
                )
                ?? throw new Exception(
                    "Connection string not found."
                );
        }

        // ==========================================
        // CREATE / UPDATE RESERVATION
        // ==========================================

        public ReservationResult SaveReservation(Reservation request)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);

                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_RESERVATION", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // ==========================================
                // BOOKING
                // ==========================================

                AddParameter(cmd, "@IP_BOOKING_ID", request.bookingId);
                AddParameter(cmd, "@IP_SESSION_ID", request.sessionId);
                // ==========================================
                // ACCOMMODATION
                // ==========================================

                AddParameter(cmd, "@IP_PROPERTY_ID", request.propertyId);
                AddParameter(cmd, "@IP_ACCM_TYPE_ID", request.accmTypeId);
                AddParameter(cmd, "@IP_TARIFF_ID", request.tariffId);
                AddParameter(cmd, "@IP_OCC_CAT_ID", request.occCatId);
                AddParameter(cmd, "@IP_ROOM_ID", request.roomId);
                AddParameter(cmd, "@IP_BED_ID", request.bedId);
                // ==========================================
                // GUEST
                // ==========================================

                AddParameter(cmd, "@IP_GUEST_NM", request.guestName);
                AddParameter(cmd, "@IP_GENDER_CD", request.genderCd);
                AddParameter(cmd, "@IP_GENDER_ID", request.genderId);
                AddParameter(cmd, "@IP_MOBILE_NO", request.mobileNo);
                AddParameter(cmd, "@IP_EMAIL_ID", request.emailId);
                AddParameter(cmd, "@IP_ADDRESS", request.address);
                // ==========================================
                // PATIENT / ADMISSION
                // ==========================================

                AddParameter(cmd, "@IP_PATIENT_ID", request.patientId);
                AddParameter(cmd, "@IP_UMR_NO", request.umrNo);
                AddParameter(cmd, "@IP_ADMN_NO", request.admnNo);
                AddParameter(cmd, "@IP_RELATIONSHIP_CD", request.relationshipCd);
                AddParameter(cmd, "@IP_RELATIONSHIP_ID", request.relationshipId);
                // ==========================================
                // CORPORATE
                // ==========================================

                AddParameter(cmd, "@IP_CORP_AGREEMENT_ID", request.corpAgreementId);
                AddParameter(cmd, "@IP_CORP_PO_REF", request.corpPoRef);
                // ==========================================
                // RESERVATION DATES
                // ==========================================

                AddParameter(cmd, "@IP_EXPECTED_CHECKIN_DT", request.expectedCheckinDt);
                AddParameter(cmd, "@IP_EXPECTED_CHECKOUT_DT", request.expectedCheckoutDt);

                // ==========================================
                // REMARKS
                // ==========================================

                AddParameter(cmd, "@IP_REMARKS", request.remarks);

                AddParameter(cmd, "@IP_OCC_CAT_CD", request.occCatCd);

                // NEW: advance with the reservation (needs 04_front_office_fixes.sql)
                if (request.advanceAmount > 0)
                {
                    AddParameter(cmd, "@IP_ADVANCE_AMT", request.advanceAmount);
                    AddParameter(cmd, "@IP_ADVANCE_MODE", request.advanceMode);
                    AddParameter(cmd, "@IP_ADVANCE_REF", string.IsNullOrWhiteSpace(request.advanceRef) ? null : request.advanceRef.Trim());
                }
                // ==========================================
                // OUTPUT PARAMETERS
                // ==========================================

                SqlParameter outBookingId = new SqlParameter("@OUT_BOOKING_ID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outBookingId);
                SqlParameter outBookingNo = new SqlParameter("@OUT_BOOKING_NO", SqlDbType.VarChar, 30)
                {
                    Direction = ParameterDirection.Output
                };

                cmd.Parameters.Add(outBookingNo);
                SqlParameter outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outStatus);
                SqlParameter outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outMessage);
                // ==========================================
                // EXECUTE
                // ==========================================

                conn.Open();
                Suvarna.DGMS.Api.Helpers.ProcParameterFilter.RemoveUnknown(cmd);   // works with the old procedure too (04 not run yet)
                cmd.ExecuteNonQuery();
                return new ReservationResult
                {
                    bookingId = outBookingId.Value == DBNull.Value ? 0 : Convert.ToInt32(outBookingId.Value),
                    bookingNo = Convert.ToString(outBookingNo.Value) ?? "",
                    status = Convert.ToString(outStatus.Value) ?? "",
                    message = Convert.ToString(outMessage.Value) ?? ""
                };
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error while saving reservation:{ex.Message}",
                    ex
                );
            }
        }
        // ==========================================
        // GET RESERVATIONS FOR GRID
        // ==========================================

        public List<ReservationListItem> GetReservations(int sessionId)
        {
            List<ReservationListItem> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_RESERVATIONS", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

                conn.Open();

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new ReservationListItem
                    {
                        occCatId = reader.SafeGetInt("occCatId"),
                        occCatName = reader.SafeGetString("occCatName"),

                        relationshipId = reader.SafeGetInt("relationshipId"),
                        relationshipName = reader.SafeGetString("relationshipName"),

                        patientName = reader.SafeGetString("patientName"),
                        patientGender = reader.SafeGetString("patientGender"),
                        umrNo = reader.SafeGetString("umrNo"),
                        admnNo = reader.SafeGetString("admnNo"),

                        guestName = reader.SafeGetString("guestName"),
                        gender = reader.SafeGetString("gender"),
                        genderId = reader.SafeGetInt("genderId"),
                        mobileNo = reader.SafeGetString("mobileNo"),
                        email = reader.SafeGetString("email"),
                        address = reader.SafeGetString("address"),

                        bookingId = reader.SafeGetInt("bookingId"),
                        bookingNo = reader.SafeGetString("bookingNo"),

                        propertyId = reader.SafeGetInt("propertyId"),
                        propertyName = reader.SafeGetString("propertyName"),

                        roomId = reader.SafeGetInt("roomId"),
                        roomNo = reader.SafeGetString("roomNo"),

                        bedId = reader.SafeGetInt("bedId"),
                        bedNo = reader.SafeGetString("bedNo"),

                        accmTypeId = reader.SafeGetInt("accmTypeId"),
                        accmTypeName = reader.SafeGetString("accmTypeName"),

                        tariffId = reader.SafeGetInt("tariffId"),
                        tariffName = reader.SafeGetString("tariffName"),

                        expectedCheckinDt = reader.SafeGetDateTime("expectedCheckinDt"),
                        expectedCheckoutDt = reader.SafeGetDateTime("expectedCheckoutDt"),
                        nights = reader.SafeGetInt("nights"),


                        bookingStatus = reader.SafeGetString("bookingStatus"),
                        reservationDt = reader.SafeGetDateTime("reservationDt"),

                        // NEW (04 script) - read only when present
                        advanceAmount = HasColumn(reader, "advanceAmount") ? (reader.SafeGetDecimal("advanceAmount") ?? 0) : 0,
                        depositAmount = HasColumn(reader, "advanceAmount") ? reader.SafeGetDecimal("advanceAmount") : null,
                        corpAgreementId = HasColumn(reader, "corpAgreementId") && !reader.IsDBNull(reader.GetOrdinal("corpAgreementId"))
                            ? reader.SafeGetInt("corpAgreementId") : null,
                        corpPoRef = HasColumn(reader, "corpPoRef") ? reader.SafeGetString("corpPoRef") : null,
                        patientId = HasColumn(reader, "patientId") && !reader.IsDBNull(reader.GetOrdinal("patientId"))
                            ? reader.SafeGetInt("patientId") : null
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while fetching reservations: {ex.Message}", ex);
            }
        }
        // ==========================================
        // CHECK RESERVATION AVAILABILITY
        // ==========================================

        public ReservationAvailability CheckAvailability(int sessionId, int propertyId, int accmTypeId, DateTime checkinDt, DateTime checkoutDt)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_CHECK_RESERVATION_AVAILABILITY", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                AddParameter(cmd, "@IP_SESSION_ID", sessionId);
                AddParameter(cmd, "@IP_PROPERTY_ID", propertyId);
                AddParameter(cmd, "@IP_ACCM_TYPE_ID", accmTypeId);
                AddParameter(cmd, "@IP_CHECKIN_DT", checkinDt);
                AddParameter(cmd, "@IP_CHECKOUT_DT", checkoutDt);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new ReservationAvailability
                    {
                        totalRooms = reader.SafeGetInt("totalRooms"),
                        blockedRooms = reader.SafeGetInt("blockedRooms"),
                        availableRooms = reader.SafeGetInt("availableRooms"),
                        isAvailable = reader["isAvailable"] != DBNull.Value && Convert.ToBoolean(reader["isAvailable"])
                    };
                }

                return new ReservationAvailability
                {
                    totalRooms = 0,
                    blockedRooms = 0,
                    availableRooms = 0,
                    isAvailable = false
                };
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error while checking room availability: {ex.Message}",
                    ex
                );
            }
        }
        public MarkNoShowResult MarkNoShowReservations(int sessionId, int? graceHours)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_MARK_NOSHOW_RESERVATIONS", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                AddParameter(cmd, "@IP_SESSION_ID", sessionId);
                AddParameter(cmd, "@IP_GRACE_HOURS", graceHours);

                var outCount = new SqlParameter("@OUT_MARKED_COUNT", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
                var outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outCount);
                cmd.Parameters.Add(outStatus);
                cmd.Parameters.Add(outMessage);

                conn.Open();
                cmd.ExecuteNonQuery();

                return new MarkNoShowResult
                {
                    markedCount = outCount.Value == DBNull.Value ? 0 : Convert.ToInt32(outCount.Value),
                    status = Convert.ToString(outStatus.Value) ?? "",
                    message = Convert.ToString(outMessage.Value) ?? ""
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while marking no-show reservations: {ex.Message}", ex);
            }
        }


        // ==========================================
        // NEW: CANCEL RESERVATION (advance refunded less the cancellation charge)
        // ==========================================

        public ReservationCancelResult CancelReservation(ReservationCancelRequest request)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_CANCEL_RESERVATION", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                AddParameter(cmd, "@IP_SESSION_ID", request.sessionId);
                AddParameter(cmd, "@IP_BOOKING_ID", request.bookingId);
                AddParameter(cmd, "@IP_REASON", request.cancelRemarks?.Trim());
                AddParameter(cmd, "@IP_CANCEL_CHARGE_AMT", request.cancelChargeAmount);
                AddParameter(cmd, "@IP_REFUND_MODE", string.IsNullOrWhiteSpace(request.refundMode) ? null : request.refundMode);
                AddParameter(cmd, "@IP_REFUND_REF", string.IsNullOrWhiteSpace(request.refundRef) ? null : request.refundRef.Trim());

                var outRefund = new SqlParameter("@OUT_REFUND_AMT", SqlDbType.Decimal) { Precision = 18, Scale = 2, Direction = ParameterDirection.Output };
                var outForfeit = new SqlParameter("@OUT_FORFEITED_AMT", SqlDbType.Decimal) { Precision = 18, Scale = 2, Direction = ParameterDirection.Output };
                var outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
                var outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outRefund);
                cmd.Parameters.Add(outForfeit);
                cmd.Parameters.Add(outStatus);
                cmd.Parameters.Add(outMessage);

                conn.Open();
                cmd.ExecuteNonQuery();

                return new ReservationCancelResult
                {
                    bookingId = request.bookingId,
                    refundAmount = outRefund.Value == DBNull.Value ? 0 : Convert.ToDecimal(outRefund.Value),
                    forfeitedAmount = outForfeit.Value == DBNull.Value ? 0 : Convert.ToDecimal(outForfeit.Value),
                    status = Convert.ToString(outStatus.Value) ?? "",
                    message = Convert.ToString(outMessage.Value) ?? ""
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while cancelling reservation: {ex.Message}", ex);
            }
        }

        private static bool HasColumn(SqlDataReader reader, string column)
        {
            for (int i = 0; i < reader.FieldCount; i++)
                if (string.Equals(reader.GetName(i), column, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        // ==========================================
        // COMMON PARAMETER HELPER
        // ==========================================

        // ==========================================
        // 03-Nov: RETURNING GUEST + DUPLICATE CHECK BY MOBILE
        // ==========================================

        public ReservationGuestCheck GetGuestCheck(int sessionId, string mobileNo, DateTime? checkinDt, DateTime? checkoutDt, int bookingId)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_RESERVATION_GUEST_CHECK", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                AddParameter(cmd, "@IP_SESSION_ID", sessionId);
                AddParameter(cmd, "@IP_MOBILE_NO", mobileNo);
                AddParameter(cmd, "@IP_CHECKIN_DT", checkinDt);
                AddParameter(cmd, "@IP_CHECKOUT_DT", checkoutDt);
                AddParameter(cmd, "@IP_BOOKING_ID", bookingId);
                conn.Open();

                var result = new ReservationGuestCheck();
                using SqlDataReader reader = cmd.ExecuteReader();

                // 1: last stay of this mobile (0 or 1 row)
                if (reader.Read())
                {
                    result.lastGuest = new ReturningGuest
                    {
                        guestName = reader.SafeGetString("guestName"),
                        genderId = reader["genderId"] == DBNull.Value ? null : Convert.ToInt32(reader["genderId"]),
                        genderCd = reader["genderCd"] == DBNull.Value ? null : reader["genderCd"].ToString(),
                        email = reader["email"] == DBNull.Value ? null : reader["email"].ToString(),
                        address = reader["address"] == DBNull.Value ? null : reader["address"].ToString(),
                        lastStayDt = reader.SafeGetDateTime("lastStayDt"),
                        stayCount = reader.SafeGetInt("stayCount")
                    };
                }

                // 2: reservations / stays of this mobile overlapping the dates
                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        result.overlapping.Add(new OverlappingBooking
                        {
                            bookingId = reader.SafeGetInt("bookingId"),
                            bookingNo = reader.SafeGetString("bookingNo"),
                            bookingStatus = reader.SafeGetString("bookingStatus"),
                            guestName = reader.SafeGetString("guestName"),
                            accmTypeName = reader["accmTypeName"] == DBNull.Value ? null : reader["accmTypeName"].ToString(),
                            checkinDt = reader.SafeGetDateTime("checkinDt"),
                            checkoutDt = reader.SafeGetDateTime("checkoutDt")
                        });
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while checking the guest mobile: {ex.Message}", ex);
            }
        }

        private static void AddParameter(
            SqlCommand cmd,
            string parameterName,
            object? value)
        {
            cmd.Parameters.AddWithValue(
                parameterName,
                value ?? DBNull.Value
            );
        }
    }
}