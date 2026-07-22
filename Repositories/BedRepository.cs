using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class BedRepository
    {
        private readonly string _conn;

        public BedRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }
        public string Save(Bed bed)
        {
            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_SAVE_BED_MAST", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IP_SESSION_ID", bed.sessionId);

            cmd.Parameters.AddWithValue("@IP_MODE", bed.mode);

            cmd.Parameters.AddWithValue(
                "@IP_BED_ID",
                bed.bedId == 0 ? DBNull.Value : bed.bedId
            );

            cmd.Parameters.AddWithValue("@IP_PROPERTY_ID", bed.propertyId);

            cmd.Parameters.AddWithValue("@IP_BUILDING_ID", bed.buildingId);

            cmd.Parameters.AddWithValue("@IP_FLOOR_ID", bed.floorId);

            cmd.Parameters.AddWithValue("@IP_ROOM_ID", bed.roomId);

            cmd.Parameters.AddWithValue("@IP_BED_NO", bed.bedNo);

            cmd.Parameters.AddWithValue("@IP_BED_TYPE_CD", bed.bedType);

            cmd.Parameters.AddWithValue("@IP_SORT_ORDER", bed.sortOrder);

            cmd.Parameters.AddWithValue("@IP_BED_STATUS_CD", bed.bedStatus);

            SqlParameter outBedId =
                new SqlParameter("@OUT_BED_ID", SqlDbType.Int);

            outBedId.Direction = ParameterDirection.Output;

            cmd.Parameters.Add(outBedId);

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

            return outMsg.Value?.ToString() ?? string.Empty;
        }
        #region Get All Beds

        public List<Bed> GetAllBeds()
        {
            List<Bed> list = new();

            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_BED_LIST", conn);


            cmd.CommandType = CommandType.StoredProcedure;

            conn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Bed
                {
                    bedId = Convert.ToInt32(dr["BED_ID"]),

                    roomId = Convert.ToInt32(dr["ROOM_ID"]),
                    propertyId = Convert.ToInt32(dr["PROPERTY_ID"]),

                    bedNo = dr["BED_NO"]?.ToString(),
                    bedType = dr["BED_TYPE_CD"]?.ToString(),
                    bedStatus = dr["BED_STATUS_CD"]?.ToString(),

                    sortOrder = Convert.ToInt16(dr["SORT_ORDER"]),
                    roomName = (dr["ROOM_NM"].ToString())
                });
            }

            return list;
        }

        #endregion

        #region Get By Id

        public Bed? GetBedById(int id)
        {
            Bed? bed = null;

            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd =
                new SqlCommand("DGMS.PR_DGMS_GET_BED_BY_ID", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IP_BED_ID", id);

            conn.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                bed = new Bed
                {
                    bedId = Convert.ToInt32(dr["bedId"]),

                    grpId = Convert.ToInt16(dr["grpId"]),
                    orgId = Convert.ToInt16(dr["orgId"]),
                    locId = Convert.ToInt16(dr["locId"]),

                    roomId = Convert.ToInt32(dr["roomId"]),
                    propertyId = Convert.ToInt32(dr["propertyId"]),

                    bedNo = dr["bedNo"].ToString(),
                    bedType = dr["bedType"].ToString(),
                    bedStatus = dr["bedStatus"].ToString(),

                    sortOrder = Convert.ToInt16(dr["sortOrder"])
                };
            }

            conn.Close();

            return bed;
        }

        #endregion

        #region Update Status

        public void UpdateStatus(Bed bed)
        {
            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd =
                new SqlCommand("DGMS.PR_DGMS_UPDATE_BED_STATUS", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IP_SESSION_ID", bed.sessionId);

            cmd.Parameters.AddWithValue("@IP_BED_ID", bed.bedId);

            cmd.Parameters.AddWithValue("@IP_BED_STATUS", bed.bedStatus);

            conn.Open();

            cmd.ExecuteNonQuery();
        }

        #endregion
    }
}