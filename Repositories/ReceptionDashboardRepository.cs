using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class ReceptionDashboardRepository
    {
        private readonly string _conn;

        public ReceptionDashboardRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        // ==========================================
        // SUMMARY
        // ==========================================

        public ReceptionDashboardSummary GetSummary(int sessionId)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_RECEPTION_DASHBOARD_SUMMARY", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return new ReceptionDashboardSummary
                    {
                        totalRooms = GetInt(dr, "TOTAL_ROOMS"),
                        occupiedRooms = GetInt(dr, "OCCUPIED_ROOMS"),
                        availableRooms = GetInt(dr, "AVAILABLE_ROOMS"),
                        pendingCheckin = GetInt(dr, "PENDING_CHECKIN"),
                        overdueCheckin = GetInt(dr, "OVERDUE_CHECKIN"),
                        dueCheckout = GetInt(dr, "DUE_CHECKOUT"),
                        overdueCheckout = GetInt(dr, "OVERDUE_CHECKOUT"),
                        reservations = GetInt(dr, "RESERVATIONS"),
                        arrivingTomorrow = GetInt(dr, "ARRIVING_TOMORROW"),
                        occupancyRate = GetDecimal(dr, "OCCUPANCY_RATE"),
                        todayRevenue = GetDecimal(dr, "TODAY_REVENUE"),
                        mtdRevenue = GetDecimal(dr, "MTD_REVENUE")
                    };
                }

                return new ReceptionDashboardSummary();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching reception dashboard summary.", ex);
            }
        }

        // ==========================================
        // ACTIVE BOOKINGS
        // ==========================================

        public List<ReceptionActiveBooking> GetActiveBookings(int sessionId)
        {
            List<ReceptionActiveBooking> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_RECEPTION_ACTIVE_BOOKINGS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new ReceptionActiveBooking
                    {
                        bookingId = GetInt(dr, "BOOKING_ID"),
                        bookingRef = GetString(dr, "BOOKING_REF") ?? "",
                        guestName = GetString(dr, "GUEST_NAME") ?? "",
                        mobileNo = GetString(dr, "MOBILE_NO") ?? "",
                        roomNo = GetString(dr, "ROOM_NO") ?? "",
                        categoryName = GetString(dr, "CATEGORY_NAME") ?? "",
                        checkinDt = GetNullableDateTime(dr, "CHECKIN_DT"),
                        expectedCheckoutDt = GetNullableDateTime(dr, "EXPECTED_CHECKOUT_DT"),
                        chargedAmount = GetDecimal(dr, "CHARGED_AMOUNT"),
                        gstPercent = GetDecimal(dr, "GST_PERCENT"),
                        depositAmount = GetDecimal(dr, "DEPOSIT_AMOUNT"),
                        netDueAmount = GetDecimal(dr, "NET_DUE_AMOUNT"),
                        // 03-Nov
                        roomId = GetOptInt(dr, "ROOM_ID"),
                        bedNo = GetOptString(dr, "BED_NO"),
                        totalBeds = GetOptInt(dr, "TOTAL_BEDS"),
                        isOverdue = GetOptString(dr, "IS_OVERDUE") == "Y"
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching active bookings.", ex);
            }
        }

        // ==========================================
        // ROOM STATUS
        // ==========================================

        public List<ReceptionRoomStatus> GetRoomStatus(int sessionId)
        {
            List<ReceptionRoomStatus> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_RECEPTION_ROOM_STATUS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new ReceptionRoomStatus
                    {
                        roomId = GetInt(dr, "ROOM_ID"),
                        roomNo = GetString(dr, "ROOM_NO") ?? "",
                        roomName = GetString(dr, "ROOM_NAME"),
                        roomStatus = GetString(dr, "ROOM_STATUS") ?? "",
                        buildingName = GetOptString(dr, "BUILDING_NAME"),
                        floorName = GetOptString(dr, "FLOOR_NAME"),
                        // 03-Nov (optional: old proc has no such columns)
                        floorId = GetOptInt(dr, "FLOOR_ID"),
                        floorSeq = GetOptInt(dr, "FLOOR_SEQ"),
                        accmTypeId = GetOptInt(dr, "ACCM_TYPE_ID"),
                        accmTypeName = GetOptString(dr, "ACCM_TYPE_NAME"),
                        totalBeds = GetOptInt(dr, "TOTAL_BEDS"),
                        maxOccupants = GetOptInt(dr, "MAX_OCCUPANTS"),
                        genderRestrict = GetOptString(dr, "GENDER_RESTRICT"),
                        occupiedBeds = GetOptInt(dr, "OCCUPIED_BEDS"),
                        guestCount = GetOptInt(dr, "GUEST_COUNT"),
                        guestName = GetOptString(dr, "GUEST_NAME"),
                        bookingId = GetOptInt(dr, "BOOKING_ID"),
                        expectedCheckoutDt = GetOptDateTime(dr, "EXPECTED_CHECKOUT_DT"),
                        isOverdue = GetOptString(dr, "IS_OVERDUE") == "Y",
                        hasDue = GetOptString(dr, "HAS_DUE") == "Y",
                        dueAmount = GetOptDecimal(dr, "DUE_AMOUNT"),
                        nextResBookingId = GetOptInt(dr, "NEXT_RES_BOOKING_ID"),
                        nextResBookingNo = GetOptString(dr, "NEXT_RES_BOOKING_NO"),
                        nextResGuestName = GetOptString(dr, "NEXT_RES_GUEST_NAME"),
                        nextResCheckinDt = GetOptDateTime(dr, "NEXT_RES_CHECKIN_DT"),
                        hkTaskId = GetOptInt(dr, "HK_TASK_ID"),
                        hkStatus = GetOptString(dr, "HK_STATUS"),
                        hkPriority = GetOptString(dr, "HK_PRIORITY"),
                        hkStaffName = GetOptString(dr, "HK_STAFF_NAME"),
                        hkTriggeredDt = GetOptDateTime(dr, "HK_TRIGGERED_DT"),
                        hkAssignedDt = GetOptDateTime(dr, "HK_ASSIGNED_DT")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching room status.", ex);
            }
        }

        // ==========================================
        // ALERTS
        // ==========================================

        public List<ReceptionAlert> GetAlerts(int sessionId)
        {
            List<ReceptionAlert> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_RECEPTION_ALERTS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new ReceptionAlert
                    {
                        alertId = GetInt(dr, "ALERT_ID"),
                        alertCode = GetString(dr, "ALERT_CODE") ?? "",
                        severity = GetString(dr, "SEVERITY") ?? "",
                        alertCount = GetInt(dr, "ALERT_COUNT"),
                        alertColor = GetString(dr, "ALERT_COLOR") ?? "",
                        alertTitle = GetString(dr, "ALERT_TITLE") ?? "",
                        alertSubtitle = GetString(dr, "ALERT_SUBTITLE") ?? "",
                        actionText = GetString(dr, "ACTION_TEXT") ?? ""
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching alerts.", ex);
            }
        }

        // ==========================================
        // REVENUE
        // ==========================================

        public List<ReceptionRevenueItem> GetRevenue(int sessionId)
        {
            List<ReceptionRevenueItem> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_RECEPTION_REVENUE", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new ReceptionRevenueItem
                    {
                        dayName = GetString(dr, "DAY_NAME") ?? "",
                        amount = GetDecimal(dr, "AMOUNT")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching revenue.", ex);
            }
        }

        // ==========================================
        // 03-Nov: ROOM POPUP
        // ==========================================

        public ReceptionRoomDetail GetRoomDetail(int sessionId, int roomId)
        {
            var result = new ReceptionRoomDetail();
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_RECEPTION_ROOM_DETAIL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_ROOM_ID", roomId);
                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                    result.beds.Add(new ReceptionRoomBed
                    {
                        bedId = GetInt(dr, "BED_ID"),
                        bedNo = GetString(dr, "BED_NO") ?? "",
                        bedName = GetString(dr, "BED_NAME"),
                        bedStatus = GetString(dr, "BED_STATUS")
                    });

                if (dr.NextResult())
                    while (dr.Read())
                        result.guests.Add(new ReceptionRoomGuest
                        {
                            bookingId = GetInt(dr, "BOOKING_ID"),
                            bookingNo = GetString(dr, "BOOKING_NO") ?? "",
                            bedId = GetInt(dr, "BED_ID"),
                            guestName = GetString(dr, "GUEST_NAME"),
                            mobileNo = GetString(dr, "MOBILE_NO"),
                            genderCd = GetString(dr, "GENDER_CD"),
                            checkinDt = GetNullableDateTime(dr, "CHECKIN_DT"),
                            expectedCheckoutDt = GetNullableDateTime(dr, "EXPECTED_CHECKOUT_DT"),
                            isOverdue = GetString(dr, "IS_OVERDUE") == "Y",
                            corpName = GetString(dr, "CORP_NAME"),
                            occCatName = GetString(dr, "OCC_CAT_NAME"),
                            netDueAmount = GetDecimal(dr, "NET_DUE_AMOUNT")
                        });

                if (dr.NextResult())
                    while (dr.Read())
                        result.reservations.Add(new ReceptionRoomReservation
                        {
                            bookingId = GetInt(dr, "BOOKING_ID"),
                            bookingNo = GetString(dr, "BOOKING_NO") ?? "",
                            bedId = GetInt(dr, "BED_ID"),
                            guestName = GetString(dr, "GUEST_NAME"),
                            mobileNo = GetString(dr, "MOBILE_NO"),
                            expectedCheckinDt = GetNullableDateTime(dr, "EXPECTED_CHECKIN_DT"),
                            expectedCheckoutDt = GetNullableDateTime(dr, "EXPECTED_CHECKOUT_DT"),
                            guestCount = GetInt(dr, "GUEST_COUNT")
                        });

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching room details.", ex);
            }
        }

        // ==========================================
        // 03-Nov: TODAY
        // ==========================================

        public List<ReceptionTodayItem> GetToday(int sessionId)
        {
            List<ReceptionTodayItem> list = new();
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_RECEPTION_TODAY", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                    list.Add(new ReceptionTodayItem
                    {
                        listType = GetString(dr, "LIST_TYPE") ?? "",
                        bookingId = GetInt(dr, "BOOKING_ID"),
                        bookingNo = GetString(dr, "BOOKING_NO") ?? "",
                        guestName = GetString(dr, "GUEST_NAME"),
                        mobileNo = GetString(dr, "MOBILE_NO"),
                        genderCd = GetString(dr, "GENDER_CD"),
                        roomId = GetInt(dr, "ROOM_ID"),
                        roomNo = GetString(dr, "ROOM_NO"),
                        bedNo = GetString(dr, "BED_NO"),
                        accmTypeId = GetInt(dr, "ACCM_TYPE_ID"),
                        accmTypeName = GetString(dr, "ACCM_TYPE_NAME"),
                        expectedDt = GetNullableDateTime(dr, "EXPECTED_DT"),
                        isLate = GetString(dr, "IS_LATE") == "Y",
                        guestCount = GetInt(dr, "GUEST_COUNT"),
                        netDueAmount = GetDecimal(dr, "NET_DUE_AMOUNT")
                    });
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching today's arrivals and departures.", ex);
            }
        }

        // ==========================================
        // 03-Nov: GLOBAL SEARCH
        // ==========================================

        public List<ReceptionSearchItem> Search(int sessionId, string search)
        {
            List<ReceptionSearchItem> list = new();
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_SEARCH_RECEPTION_GUESTS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.Add("@IP_SEARCH", SqlDbType.NVarChar, 100).Value = search ?? "";
                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (dr.FieldCount < 2) break;   // search text too short
                    list.Add(new ReceptionSearchItem
                    {
                        bookingId = GetInt(dr, "BOOKING_ID"),
                        bookingNo = GetString(dr, "BOOKING_NO") ?? "",
                        bookingStatus = GetString(dr, "BOOKING_STATUS") ?? "",
                        guestName = GetString(dr, "GUEST_NAME"),
                        mobileNo = GetString(dr, "MOBILE_NO"),
                        umrNo = GetString(dr, "UMR_NO"),
                        roomId = GetInt(dr, "ROOM_ID"),
                        roomNo = GetString(dr, "ROOM_NO"),
                        bedNo = GetString(dr, "BED_NO"),
                        expectedCheckinDt = GetNullableDateTime(dr, "EXPECTED_CHECKIN_DT"),
                        expectedCheckoutDt = GetNullableDateTime(dr, "EXPECTED_CHECKOUT_DT")
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while searching guests.", ex);
            }
        }

        // ==========================================
        // 03-Nov: BEST ROOM FOR AN ARRIVAL
        // ==========================================

        public List<ReceptionRoomSuggestion> SuggestRooms(int sessionId, int bookingId)
        {
            List<ReceptionRoomSuggestion> list = new();
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_SUGGEST_RECEPTION_ROOMS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_BOOKING_ID", bookingId);
                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (dr.FieldCount < 2) break;   // reservation not found
                    list.Add(new ReceptionRoomSuggestion
                    {
                        roomId = GetInt(dr, "ROOM_ID"),
                        roomNo = GetString(dr, "ROOM_NO") ?? "",
                        floorName = GetString(dr, "FLOOR_NAME"),
                        accmTypeName = GetString(dr, "ACCM_TYPE_NAME"),
                        roomStatus = GetString(dr, "ROOM_STATUS"),
                        totalBeds = GetInt(dr, "TOTAL_BEDS"),
                        maxOccupants = GetInt(dr, "MAX_OCCUPANTS"),
                        freeBeds = GetInt(dr, "FREE_BEDS"),
                        bedId = GetInt(dr, "BED_ID"),
                        bedNo = GetString(dr, "BED_NO"),
                        genderRestrict = GetString(dr, "GENDER_RESTRICT"),
                        freeUntilDt = GetNullableDateTime(dr, "FREE_UNTIL_DT"),
                        fit = GetInt(dr, "FIT"),
                        reason = GetString(dr, "REASON"),
                        guestCount = GetInt(dr, "GUEST_COUNT")
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while finding rooms for the reservation.", ex);
            }
        }

        // ==========================================
        // 03-Nov: DIRTY ROOM -> HOUSEKEEPING PRIORITY
        // ==========================================

        public (string status, string message) SetHkPriority(int sessionId, int roomId)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_SET_HK_ROOM_PRIORITY", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_ROOM_ID", roomId);
                var st = cmd.Parameters.Add("@OUT_STATUS", SqlDbType.Char, 1); st.Direction = ParameterDirection.Output;
                var msg = cmd.Parameters.Add("@OUT_MESSAGE", SqlDbType.NVarChar, 500); msg.Direction = ParameterDirection.Output;
                conn.Open();
                cmd.ExecuteNonQuery();
                return (Convert.ToString(st.Value) ?? "E", Convert.ToString(msg.Value) ?? "");
            }
            catch (Exception ex)
            {
                throw new Exception("Error while setting housekeeping priority.", ex);
            }
        }

        // ==========================================
        // 03-Nov: SHIFT HANDOVER
        // ==========================================

        public ShiftHandover GetShiftHandover(int sessionId)
        {
            var result = new ShiftHandover();
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_SHIFT_HANDOVER", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result.shiftName = GetString(dr, "SHIFT_NAME") ?? "";
                    result.shiftStartDt = GetNullableDateTime(dr, "SHIFT_START_DT");
                    result.shiftEndDt = GetNullableDateTime(dr, "SHIFT_END_DT");
                    result.userName = GetString(dr, "USER_NAME");
                    result.checkinCount = GetInt(dr, "CHECKIN_CNT");
                    result.checkoutCount = GetInt(dr, "CHECKOUT_CNT");
                    result.reservationCount = GetInt(dr, "RESERVATION_CNT");
                    result.cashAmount = GetDecimal(dr, "CASH_AMT");
                    result.upiAmount = GetDecimal(dr, "UPI_AMT");
                    result.cardAmount = GetDecimal(dr, "CARD_AMT");
                    result.otherAmount = GetDecimal(dr, "OTHER_AMT");
                    result.totalAmount = GetDecimal(dr, "TOTAL_AMT");
                    result.arrivalsPending = GetInt(dr, "ARRIVALS_PENDING");
                    result.dirtyRooms = GetInt(dr, "DIRTY_ROOMS");
                    result.maintenanceRooms = GetInt(dr, "MAINTENANCE_ROOMS");
                }

                if (dr.NextResult())
                    while (dr.Read())
                        result.overdueGuests.Add(new ShiftOverdueGuest
                        {
                            bookingId = GetInt(dr, "BOOKING_ID"),
                            bookingNo = GetString(dr, "BOOKING_NO") ?? "",
                            roomNo = GetString(dr, "ROOM_NO"),
                            guestName = GetString(dr, "GUEST_NAME"),
                            expectedCheckoutDt = GetNullableDateTime(dr, "EXPECTED_CHECKOUT_DT"),
                            netDueAmount = GetDecimal(dr, "NET_DUE_AMOUNT")
                        });

                if (dr.NextResult() && dr.Read())
                    result.previousNote = new ShiftPreviousNote
                    {
                        shiftName = GetString(dr, "SHIFT_NAME"),
                        notes = GetString(dr, "NOTES"),
                        handedTo = GetString(dr, "HANDED_TO"),
                        createDt = GetNullableDateTime(dr, "CREATE_DT"),
                        createdByName = GetString(dr, "CREATED_BY_NAME")
                    };

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching shift handover.", ex);
            }
        }

        public (string status, string message) SaveShiftHandover(ShiftHandoverSaveRequest req)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_SAVE_SHIFT_HANDOVER", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", req.sessionId);
                cmd.Parameters.Add("@IP_NOTES", SqlDbType.NVarChar, 1000).Value = (object?)req.notes ?? DBNull.Value;
                cmd.Parameters.Add("@IP_HANDED_TO", SqlDbType.NVarChar, 100).Value = (object?)req.handedTo ?? DBNull.Value;
                var st = cmd.Parameters.Add("@OUT_STATUS", SqlDbType.Char, 1); st.Direction = ParameterDirection.Output;
                var msg = cmd.Parameters.Add("@OUT_MESSAGE", SqlDbType.NVarChar, 500); msg.Direction = ParameterDirection.Output;
                conn.Open();
                cmd.ExecuteNonQuery();
                return (Convert.ToString(st.Value) ?? "E", Convert.ToString(msg.Value) ?? "");
            }
            catch (Exception ex)
            {
                throw new Exception("Error while saving shift handover.", ex);
            }
        }

        // ==========================================
        // COMMON HELPERS
        // ==========================================

        private static string? GetString(SqlDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? null : Convert.ToString(dr[column]);
        }

        // column added by the 23-Oct script: null until that script is run
        private static string? GetOptString(SqlDataReader dr, string column)
        {
            for (int i = 0; i < dr.FieldCount; i++)
                if (string.Equals(dr.GetName(i), column, StringComparison.OrdinalIgnoreCase))
                    return dr.IsDBNull(i) ? null : Convert.ToString(dr[i]);
            return null;
        }

        private static int Ordinal(SqlDataReader dr, string column)
        {
            for (int i = 0; i < dr.FieldCount; i++)
                if (string.Equals(dr.GetName(i), column, StringComparison.OrdinalIgnoreCase)) return i;
            return -1;
        }

        private static int GetOptInt(SqlDataReader dr, string column)
        {
            int i = Ordinal(dr, column);
            return i < 0 || dr.IsDBNull(i) ? 0 : Convert.ToInt32(dr[i]);
        }

        private static decimal GetOptDecimal(SqlDataReader dr, string column)
        {
            int i = Ordinal(dr, column);
            return i < 0 || dr.IsDBNull(i) ? 0 : Convert.ToDecimal(dr[i]);
        }

        private static DateTime? GetOptDateTime(SqlDataReader dr, string column)
        {
            int i = Ordinal(dr, column);
            return i < 0 || dr.IsDBNull(i) ? null : Convert.ToDateTime(dr[i]);
        }

        private static int GetInt(SqlDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? 0 : Convert.ToInt32(dr[column]);
        }

        private static decimal GetDecimal(SqlDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? 0 : Convert.ToDecimal(dr[column]);
        }

        private static DateTime? GetNullableDateTime(SqlDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? null : Convert.ToDateTime(dr[column]);
        }
    }
}
