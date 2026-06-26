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

        #region Get All Beds

        public List<Bed> GetAllBeds()
        {
            List<Bed> list = new();

            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_BED_LIST", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            conn.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Bed
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
                });
            }

            conn.Close();

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