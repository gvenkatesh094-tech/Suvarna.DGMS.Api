using Suvarna.DGMS.Api.Helpers;
using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class FloorRepository
    {
        private readonly SessionContext _session;
        private readonly string _conn;

        public FloorRepository(IConfiguration configuration, SessionContext session)
        {
            _session = session;
            _conn = configuration.GetConnectionString("SqlServer");
        }

        public void SaveFloor(Floor floor)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.pr_insupd_dgm_floor", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MODE", floor.mode);

                cmd.Parameters.AddWithValue("@FLOOR_ID",
                    floor.floorId == 0 ? DBNull.Value : (object)floor.floorId);

                cmd.Parameters.AddWithValue("@BUILDING_ID",
                    floor.buildingId == 0 ? DBNull.Value : (object)floor.buildingId);

                cmd.Parameters.AddWithValue("@PROPERTY_ID",
                    floor.propertyId == 0 ? DBNull.Value : (object)floor.propertyId);

                cmd.Parameters.AddWithValue("@FLOOR_CD",
                    (object?)floor.floorCode ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@FLOOR_NM",
                    (object?)floor.floorName ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@BUILDING_NAME",
                    (object?)floor.buildingName ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@FLOOR_SEQ", floor.floorSeq);

                cmd.Parameters.AddWithValue("@REMARKS",
                    (object?)floor.remarks ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@STATUS",
                    (object?)floor.status ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@CREATE_BY", floor.userId);
                cmd.Parameters.AddWithValue("@MODIFY_BY", floor.userId);

                cmd.Parameters.AddWithValue("@GRP_ID", floor.grpId);
                cmd.Parameters.AddWithValue("@ORG_ID", floor.orgId);
                cmd.Parameters.AddWithValue("@LOC_ID", floor.locId);
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", floor.sessionId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while saving floor.", ex);
            }
        }
        


        public List<Floor> GetAllFloors()
        {
            List<Floor> list = new();

            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd = new SqlCommand("DGMS.pr_get_dgm_floor", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IP_SESSION_ID", _session.Require());

            conn.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Floor
                {
                    floorId = Convert.ToInt32(dr["floorId"]),                  
                    buildingId = Convert.ToInt32(dr["buildingId"]),
                    propertyId = Convert.ToInt32(dr["propertyId"]),
                    floorCode = dr["floorCode"].ToString(),
                    floorName = dr["floorName"].ToString(),
                    buildingName = dr["buildingName"].ToString(),
                    floorSeq = Convert.ToInt16(dr["floorSeq"]),
                    remarks = dr["remarks"] == DBNull.Value ? null : dr["remarks"].ToString(),
                    status = dr["status"] == DBNull.Value ? null : dr["status"].ToString(),
                    grpId = Convert.ToInt16(dr["grpId"]),
                    orgId = Convert.ToInt16(dr["orgId"]),
                    locId = Convert.ToInt16(dr["locId"]),
                });
            }

            conn.Close();

            return list;
        }

    

        

    }
}
