using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class FloorRepository
    {
        private readonly string _conn;

        public FloorRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }


        public void InsertFloor(Floor floor)
        {
            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.pr_ins_dgm_floor", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@BUILDING_ID", floor.buildingId);
            cmd.Parameters.AddWithValue("@PROPERTY_ID", floor.propertyId);
            cmd.Parameters.AddWithValue("@FLOOR_CD", floor.floorCode);
            cmd.Parameters.AddWithValue("@FLOOR_NM", floor.floorName);
            cmd.Parameters.AddWithValue("@BUILDING_NAME", floor.buildingName);
            cmd.Parameters.AddWithValue("@FLOOR_SEQ", floor.floorSeq);
            cmd.Parameters.AddWithValue("@REMARKS", (object?)floor.remarks ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@STATUS", floor.status);
            cmd.Parameters.AddWithValue("@CREATE_BY", floor.userId);
            cmd.Parameters.AddWithValue("@GRP_ID", floor.grpId);
            cmd.Parameters.AddWithValue("@ORG_ID", floor.orgId);
            cmd.Parameters.AddWithValue("@LOC_ID", floor.locId);

            conn.Open();
            cmd.ExecuteNonQuery();
        }


        public void UpdateFloor(Floor floor)
        {
            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.pr_upd_dgm_floor", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@FLOOR_ID", floor.floorId);

            cmd.Parameters.AddWithValue("@BUILDING_ID", floor.buildingId);
            cmd.Parameters.AddWithValue("@PROPERTY_ID", floor.propertyId);

            cmd.Parameters.AddWithValue("@FLOOR_CD", floor.floorCode);
            cmd.Parameters.AddWithValue("@FLOOR_NM", floor.floorName);
            cmd.Parameters.AddWithValue("@BUILDING_NAME", floor.buildingName);
            cmd.Parameters.AddWithValue("@FLOOR_SEQ", floor.floorSeq);

            cmd.Parameters.AddWithValue("@REMARKS", (object?)floor.remarks ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@STATUS", floor.status);
            cmd.Parameters.AddWithValue("@MODIFY_BY", floor.userId);
            cmd.Parameters.AddWithValue("@GRP_ID", floor.grpId);
            cmd.Parameters.AddWithValue("@ORG_ID", floor.orgId);
            cmd.Parameters.AddWithValue("@LOC_ID", floor.locId);

            conn.Open();
            cmd.ExecuteNonQuery();
        }
        public void DeleteFloor(DeleteFloorRequest floor)
        {
            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd =
                new SqlCommand("DGMS.pr_del_dgm_floor", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue(
                "@FLOOR_ID",
                floor.floorId);

            conn.Open();

            cmd.ExecuteNonQuery();
        }


        public List<Floor> GetAllFloors()
        {
            List<Floor> list = new();

            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd = new SqlCommand("DGMS.pr_get_dgm_floor", conn);

            cmd.CommandType = CommandType.StoredProcedure;

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

    

        public Floor? GetFloorById(int id)
        {
            Floor? floor = null;

            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd = new SqlCommand("DGMS.pr_get_dgm_floor_byid", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@DGM_FLOOR_MAST_ID", id);

            conn.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                floor = new Floor
                {
                    floorId = Convert.ToInt32(dr["floorId"]),
                    grpId = Convert.ToInt16(dr["grpId"]),
                    orgId = Convert.ToInt16(dr["orgId"]),
                    locId = Convert.ToInt16(dr["locId"]),

                    buildingId = Convert.ToInt32(dr["buildingId"]),
                    propertyId = Convert.ToInt32(dr["propertyId"]),

                    floorCode = dr["floorCode"].ToString(),
                    floorName = dr["floorName"].ToString(),

                    floorSeq = Convert.ToInt16(dr["floorSeq"]),

                    remarks = dr["remarks"] == DBNull.Value
                        ? null
                        : dr["remarks"].ToString()
                };
            }

            conn.Close();

            return floor;
        }

    }
}
