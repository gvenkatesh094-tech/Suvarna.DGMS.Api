using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class PropertyRepository
    {
        private readonly string _conn;

        public PropertyRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        public void InsertProperty(Property property)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.pr_ins_dgm_property", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@GRP_ID", property.grpId);
                cmd.Parameters.AddWithValue("@ORG_ID", property.orgId);
                cmd.Parameters.AddWithValue("@LOC_ID", property.locId);

                cmd.Parameters.AddWithValue("@PROPERTY_CD", property.propertyCode);
                cmd.Parameters.AddWithValue("@PROPERTY_NM", property.propertyName);
                cmd.Parameters.AddWithValue("@PROPERTY_TYPE_CD", property.propertyType);

                cmd.Parameters.AddWithValue("@ADDRESS_LINE1", (object?)property.address ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ADDRESS_LINE2", DBNull.Value);

                cmd.Parameters.AddWithValue("@CITY", (object?)property.city ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@STATE_CD", (object?)property.state ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PIN_CODE", (object?)property.pinCode ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@GSTIN", (object?)property.gstin ?? DBNull.Value);
                //cmd.Parameters.AddWithValue("@SAC_CODE", "9123113"); // NEED TO DISCUSS

                //cmd.Parameters.AddWithValue("@TOTAL_ROOMS", property.totalRooms ?? 0);
                //cmd.Parameters.AddWithValue("@TOTAL_BEDS", property.totalBeds ?? 0);

                cmd.Parameters.AddWithValue("@CONTACT_PERSON_NM", DBNull.Value);
                cmd.Parameters.AddWithValue("@CONTACT_PHONE", DBNull.Value);

                cmd.Parameters.AddWithValue("@CONTACT_EMAIL", (object?)property.email ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@CREATE_BY", property.userId);

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

            cmd.Parameters.AddWithValue("@GRP_ID", property.grpId);
            cmd.Parameters.AddWithValue("@ORG_ID", property.orgId);
            cmd.Parameters.AddWithValue("@LOC_ID", property.locId);

            cmd.Parameters.AddWithValue("@PROPERTY_CD", property.propertyCode);
            cmd.Parameters.AddWithValue("@PROPERTY_NM", property.propertyName);
            cmd.Parameters.AddWithValue("@PROPERTY_TYPE_CD", property.propertyType);

            cmd.Parameters.AddWithValue("@ADDRESS_LINE1", (object?)property.address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ADDRESS_LINE2", DBNull.Value);

            cmd.Parameters.AddWithValue("@CITY", (object?)property.city ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@STATE_CD", (object?)property.state ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PIN_CODE", (object?)property.pinCode ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@GSTIN", (object?)property.gstin ?? DBNull.Value);
            //cmd.Parameters.AddWithValue("@SAC_CODE", property.sacCode);  // need to discuss

            //cmd.Parameters.AddWithValue("@TOTAL_ROOMS", property.totalRooms ?? 0);
            //cmd.Parameters.AddWithValue("@TOTAL_BEDS", property.totalBeds ?? 0);

            cmd.Parameters.AddWithValue("@CONTACT_PERSON_NM", DBNull.Value);
            cmd.Parameters.AddWithValue("@CONTACT_PHONE", DBNull.Value);

            cmd.Parameters.AddWithValue("@CONTACT_EMAIL", (object?)property.email ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@MODIFY_BY", property.userId);

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
                    grpId = Convert.ToInt16(dr["GrpId"]),
                    orgId = Convert.ToInt16(dr["OrgId"]),
                    locId = Convert.ToInt16(dr["LocId"]),

                    propertyCode = dr["PropertyCode"].ToString(),
                    propertyName = dr["PropertyName"].ToString(),
                    propertyType = dr["PropertyType"].ToString(),

                    address = dr["Address"]?.ToString(),
                    city = dr["CITY"]?.ToString(),
                    state = dr["State"]?.ToString(),
                    pinCode = dr["PinCode"]?.ToString(),

                    gstin = dr["GSTIN"]?.ToString(),
                  //  sacCode = dr["SacCode"]?.ToString(),

                  //  totalRooms = dr["TotalRooms"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TotalRooms"]),
                  //  totalBeds = dr["TotalBeds"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TotalBeds"]),

                   // contactPersonName = dr["ContactPersonName"]?.ToString(),
                    //contactPhone = dr["ContactPhone"]?.ToString(),

                    email = dr["Email"]?.ToString()
                });
            }

            return list;
        }
    }
}