using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Helpers;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class RoomRepository
    {
        private readonly string _conn;

        public RoomRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        public string Save(Room room, string mode)
        {
            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd =
                new SqlCommand("DGMS.PR_DGMS_SAVE_ROOM_MAST", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IP_SESSION_ID", room.sessionId);
            cmd.Parameters.AddWithValue("@IP_LOCATION_ID", room.locationId);

            cmd.Parameters.AddWithValue("@IP_MODE", mode);

            cmd.Parameters.AddWithValue("@IP_ROOM_ID",
                room.roomId == 0 ? DBNull.Value : room.roomId);

            cmd.Parameters.AddWithValue("@IP_PROPERTY_ID", room.propertyId);

            cmd.Parameters.AddWithValue("@IP_BUILDING_ID", room.buildingId);

            cmd.Parameters.AddWithValue("@IP_FLOOR_ID", room.floorId);

            cmd.Parameters.AddWithValue("@IP_ACCM_TYPE_ID", room.accmTypeId);

            cmd.Parameters.AddWithValue("@IP_ROOM_NO", room.roomNo);

            cmd.Parameters.AddWithValue("@IP_ROOM_NM",
                (object?)room.roomName ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@IP_TOTAL_BEDS", room.totalBeds);

            cmd.Parameters.AddWithValue("@IP_FLOOR_AREA_SQFT",
                (object?)room.floorAreaSqft ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@IP_IS_ATTACHED_BATH",
                room.isAttachedBath);

            cmd.Parameters.AddWithValue("@IP_IS_AC",
                room.isAc);

            cmd.Parameters.AddWithValue("@IP_IS_TV",
                room.isTv);

            cmd.Parameters.AddWithValue("@IP_IS_WIFI",
                room.isWifi);

            cmd.Parameters.AddWithValue("@IP_IS_ACCESSIBLE",
                room.isAccessible);

            cmd.Parameters.AddWithValue("@IP_WARD_PROXIMITY",
                (object?)room.wardProximity ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@IP_PHONE_EXTENSION",
                (object?)room.phoneExtension ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@IP_LOCKER_COUNT",
                room.lockerCount);

            cmd.Parameters.AddWithValue("@IP_REMARKS",
                (object?)room.remarks ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IP_PRICE",
             (object?)room.roomPrice ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IP_GST_PER",
           (object?)room.gst ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IP_ROOM_STATUS",
           (object?)room.roomStatus ?? DBNull.Value);

            SqlParameter outId =
                new SqlParameter("@OUT_ROOM_ID", SqlDbType.Int);

            outId.Direction = ParameterDirection.Output;

            cmd.Parameters.Add(outId);

            SqlParameter outStatus =
                new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1);

            outStatus.Direction = ParameterDirection.Output;

            cmd.Parameters.Add(outStatus);

            SqlParameter outMsg =
                new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500);

            outMsg.Direction = ParameterDirection.Output;

            cmd.Parameters.Add(outMsg);

            conn.Open();

            cmd.ExecuteNonQuery();

            return outMsg.Value.ToString();
        }
        public List<Room> GetAllRooms(int sessionId,int locationId)
        {
            List<Room> rooms = new List<Room>();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                using (SqlCommand cmd = new SqlCommand("DGMS.PR_GET_ROOMS", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                    cmd.Parameters.AddWithValue("@IP_LOCATION_ID", locationId);

                    con.Open();

                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        rooms.Add(new Room
                        {
                            roomId = Convert.ToInt32(dr["roomId"]),
                            propertyId = Convert.ToInt32(dr["propertyId"]),
                            buildingId = Convert.ToInt32(dr["buildingId"]),
                            floorId = Convert.ToInt32(dr["floorId"]),
                            accmTypeId = Convert.ToInt32(dr["accmTypeId"]),

                            roomNo = dr["roomNo"].ToString(),
                            roomName = dr["roomName"]?.ToString(),

                            totalBeds = Convert.ToInt16(dr["totalBeds"]),

                            floorAreaSqft = dr["floorAreaSqft"] == DBNull.Value
         ? null
         : Convert.ToDecimal(dr["floorAreaSqft"]),

                            isAttachedBath = dr["isAttachedBath"].ToString(),
                            isAc = dr["isAc"].ToString(),
                            isTv = dr["isTv"].ToString(),
                            isWifi = dr["isWifi"].ToString(),
                            isAccessible = dr["isAccessible"].ToString(),

                            wardProximity = dr["wardProximity"] == DBNull.Value
         ? null
         : dr["wardProximity"].ToString(),

                            phoneExtension = dr["phoneExtension"] == DBNull.Value
         ? null
         : dr["phoneExtension"].ToString(),

                            lockerCount = Convert.ToInt16(dr["lockerCount"]),

                            remarks = dr["remarks"] == DBNull.Value
         ? null
         : dr["remarks"].ToString(),
                            roomType = dr["roomType"].ToString(),

                            roomPrice = dr["roomPrice"] == DBNull.Value
    ? 0
    : Convert.ToDecimal(dr["roomPrice"]),

                            gst = dr["gst"] == DBNull.Value
    ? 0
    : Convert.ToDecimal(dr["gst"]),

                            maxOccupants = dr["maxOccupants"] == DBNull.Value
    ? 0
    : Convert.ToInt32(dr["maxOccupants"]),

                            roomStatus = dr["roomStatus"].ToString(),
                        });
                    }
                }
            }

            return rooms;
        }

        public string GenroomCode(GenerateCodeRequest request)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("PR_GET_PAGELOAD_TABKEY", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_TABLE_NAME", request.masterName);
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", request.sessionId);

                SqlParameter code = new SqlParameter("@OP_TABKEY_CD", SqlDbType.VarChar, 20);
                code.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(code);

                conn.Open();

                cmd.ExecuteNonQuery();

                return Convert.ToString(code.Value);
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating GenroomCode .", ex);
            }
        }
    }

}
