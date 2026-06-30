using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class BuildingRepository
    {
        private readonly string _conn;

        public BuildingRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }


        #region Save

        public void InsertBuilding(Building building)
        {
            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd = new SqlCommand("DGMS.pr_ins_dgm_building", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@GRP_ID", building.grpId);
            cmd.Parameters.AddWithValue("@ORG_ID", building.orgId);
            cmd.Parameters.AddWithValue("@LOC_ID", building.locId);

            cmd.Parameters.AddWithValue("@PROPERTY_ID", building.propertyId);

            cmd.Parameters.AddWithValue("@BUILDING_CD", building.buildingCode);
            cmd.Parameters.AddWithValue("@BUILDING_NM", building.buildingName);

            cmd.Parameters.AddWithValue("@TOTAL_FLOORS", building.totalFloors);

            cmd.Parameters.AddWithValue("@REMARKS",
                (object?)building.remarks ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@CREATE_BY", building.userId);

            conn.Open();

            cmd.ExecuteNonQuery();
        }

        #endregion

        #region Update

        public void UpdateBuilding(Building building)
        {
            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd = new SqlCommand("DGMS.pr_upd_dgm_building", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@DGM_BUILDING_MAST_ID", building.buildingId);

            cmd.Parameters.AddWithValue("@GRP_ID", building.grpId);
            cmd.Parameters.AddWithValue("@ORG_ID", building.orgId);
            cmd.Parameters.AddWithValue("@LOC_ID", building.locId);

            cmd.Parameters.AddWithValue("@PROPERTY_ID", building.propertyId);

            cmd.Parameters.AddWithValue("@BUILDING_CD", building.buildingCode);
            cmd.Parameters.AddWithValue("@BUILDING_NM", building.buildingName);

            cmd.Parameters.AddWithValue("@TOTAL_FLOORS", building.totalFloors);

            cmd.Parameters.AddWithValue("@REMARKS",
                (object?)building.remarks ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@MODIFY_BY", building.userId);

            conn.Open();

            cmd.ExecuteNonQuery();
        }
        public void Deletebuilding(Building building)
        {
            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd = new SqlCommand("DGMS.pr_del_dgm_property", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PROPERTY_CD", (object?)building.buildingCode ?? DBNull.Value);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        #endregion

        #region Get All

        public List<Building> GetAllBuildings()
        {
            List<Building> list = new();

            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd = new SqlCommand("DGMS.pr_get_dgm_building", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            conn.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Building
                {
                    buildingId = Convert.ToInt32(dr["buildingId"]),
                    buildingCode = dr["buildingCode"].ToString(),
                    buildingName = dr["buildingName"].ToString(),
                    propertyId = Convert.ToInt32(dr["propertyId"]),
                    propertyName = dr["PropertyName"].ToString(), 
                    totalFloors = Convert.ToInt16(dr["totalFloors"]),
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

        #endregion

        #region Get By Id

        public Building? GetBuildingById(int id)
        {
            Building? building = null;

            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd = new SqlCommand("DGMS.pr_get_dgm_building_byid", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@DGM_BUILDING_MAST_ID", id);

            conn.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                building = new Building
                {
                    buildingId = Convert.ToInt32(dr["buildingId"]),

                    grpId = Convert.ToInt16(dr["grpId"]),
                    orgId = Convert.ToInt16(dr["orgId"]),
                    locId = Convert.ToInt16(dr["locId"]),

                    propertyId = Convert.ToInt32(dr["propertyId"]),

                    buildingCode = dr["buildingCode"].ToString(),

                    buildingName = dr["buildingName"].ToString(),

                    totalFloors = Convert.ToInt16(dr["totalFloors"]),

                    remarks = dr["remarks"] == DBNull.Value
                        ? null
                        : dr["remarks"].ToString()
                };
            }

            conn.Close();

            return building;
        }

        #endregion
    }
}