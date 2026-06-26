using Microsoft.Data.SqlClient;
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
    }

}
