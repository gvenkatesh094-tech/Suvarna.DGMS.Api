using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;
using System.Net.NetworkInformation;

namespace Suvarna.DGMS.Api.Repositories
{
    public class PropertyRepository
    {
        private readonly string _conn;

        public PropertyRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");

        }

        public string GenerateCodeProperty(GenerateCodeRequest request)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.pr_generate_dgms_property", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MASTER_NAME", request.masterName);
                cmd.Parameters.AddWithValue("@SESSION_ID", request.sessionId);

                SqlParameter code = new SqlParameter("@PROPERTY_CD", SqlDbType.VarChar, 20);
                code.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(code);

                conn.Open();

                cmd.ExecuteNonQuery();

                return Convert.ToString(code.Value);
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating Property Code.", ex);
            }
        }
        public void InsertProperty(Property property)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.pr_ins_dgm_property", conn);

                cmd.CommandType = CommandType.StoredProcedure;



                cmd.Parameters.AddWithValue("@PROPERTY_CD", property.propertyCode);
                cmd.Parameters.AddWithValue("@PROPERTY_NM", property.propertyName);
                cmd.Parameters.AddWithValue("@PROPERTY_TYPE_CD", property.propertyType);
                cmd.Parameters.AddWithValue("@ADDRESS_LINE1", (object?)property.address ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ADDRESS_LINE2", (object?)property.address ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@CITY", (object?)property.city ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@STATE_CD", (object?)property.state ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PIN_CODE", (object?)property.pinCode ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@GSTIN", (object?)property.gstin ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@TOTAL_BUILDINGS", (object?)property.buildings ?? 0);
                cmd.Parameters.AddWithValue("@TOTAL_ROOMS", (object?)property.rooms ?? 0);
                cmd.Parameters.AddWithValue("@TOTAL_BEDS", (object?)property.beds ?? 0);

                cmd.Parameters.AddWithValue("@CONTACT_PERSON_NM", (object?)property.phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CONTACT_PHONE", (object?)property.phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CONTACT_EMAIL", (object?)property.email ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@CREATE_BY", property.userId);
                cmd.Parameters.AddWithValue("@status", (object?)property.status ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@GRP_ID", property.grpId);
                cmd.Parameters.AddWithValue("@ORG_ID", property.orgId);
                cmd.Parameters.AddWithValue("@LOC_ID", property.locId);


                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting property.", ex);
            }
        }
        public void UpdateProperty(Property property)
        {
            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd = new SqlCommand("DGMS.pr_upd_dgm_property", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@DGM_PROPERTY_MAST_ID", property.propertyId);



            cmd.Parameters.AddWithValue("@PROPERTY_CD", property.propertyCode);
            cmd.Parameters.AddWithValue("@PROPERTY_NM", property.propertyName);
            cmd.Parameters.AddWithValue("@PROPERTY_TYPE_CD", property.propertyType);
            cmd.Parameters.AddWithValue("@ADDRESS_LINE1", (object?)property.address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ADDRESS_LINE2", (object?)property.address ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@CITY", (object?)property.city ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@STATE_CD", (object?)property.state ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PIN_CODE", (object?)property.pinCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@GSTIN", (object?)property.gstin ?? DBNull.Value);
            //cmd.Parameters.AddWithValue("@SAC_CODE", property.sacCode);  // need to discuss

            cmd.Parameters.AddWithValue("@TOTAL_BUILDINGS", (object?)property.buildings ?? 0);
            cmd.Parameters.AddWithValue("@TOTAL_ROOMS", (object?)property.rooms ?? 0);
            cmd.Parameters.AddWithValue("@TOTAL_BEDS", (object?)property.beds ?? 0);


            cmd.Parameters.AddWithValue("@CONTACT_PERSON_NM", (object?)property.phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CONTACT_PHONE", (object?)property.phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CONTACT_EMAIL", (object?)property.email ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@MODIFY_BY", property.userId);
            cmd.Parameters.AddWithValue("@status", (object?)property.status ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@GRP_ID", property.grpId);
            cmd.Parameters.AddWithValue("@ORG_ID", property.orgId);
            cmd.Parameters.AddWithValue("@LOC_ID", property.locId);

            conn.Open();

            cmd.ExecuteNonQuery();
        }
        public void DeleteProperty(Property property)
        {
            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd = new SqlCommand("DGMS.pr_del_dgm_property", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PROPERTY_CD", (object?)property.propertyCode ?? DBNull.Value);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        public List<Property> GetAllProperties()
        {
            List<Property> list = new List<Property>();

            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.pr_get_dgm_property", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            conn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Property
                {
                    propertyId = Convert.ToInt32(dr["PropertyId"]),


                    propertyCode = dr["PropertyCode"].ToString(),
                    propertyName = dr["PropertyName"].ToString(),
                    propertyType = dr["PropertyType"].ToString(),

                    address = dr["Address"]?.ToString(),
                    city = dr["CITY"]?.ToString(),
                    state = dr["State"]?.ToString(),
                    pinCode = dr["PinCode"]?.ToString(),
                    gstin = dr["GSTIN"]?.ToString(),
                    //sacCode = dr["SacCode"]?.ToString(),

                    buildings = dr["TotalBuildings"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TotalBuildings"]),
                    rooms = dr["TotalRooms"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TotalRooms"]),
                    beds = dr["TotalBeds"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TotalBeds"]),

                    //phone = dr["ContactPersonName"]?.ToString(),
                    phone = dr["ContactPhone"]?.ToString(),

                    email = dr["Email"]?.ToString(),
                    status = dr["status"]?.ToString(),

                    grpId = Convert.ToInt16(dr["GrpId"]),
                    orgId = Convert.ToInt16(dr["OrgId"]),
                    locId = Convert.ToInt16(dr["LocId"])
                });
            }

            return list;
        }
    }
}