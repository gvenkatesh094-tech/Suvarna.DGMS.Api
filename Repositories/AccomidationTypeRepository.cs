using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Suvarna.DGMS.Api.Helpers;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class AccomidationTypeRepository
    {
        private readonly string _conn;

        public AccomidationTypeRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        public void SaveAccommodation(AccomidationType accommodation)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_INSUPD_ACCOMMODATION_TYPE", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MODE", accommodation.mode);
                cmd.Parameters.AddWithValue("@DGM_ACCM_TYPE_MAST_ID", accommodation.accmTypeId == 0 ? DBNull.Value : (object)accommodation.accmTypeId);
                cmd.Parameters.AddWithValue("@ACCM_TYPE_CD", (object?)accommodation.accmTypeCode ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ACCM_TYPE_NM", (object?)accommodation.accmTypeName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ACCM_CATEGORY", (object?)accommodation.accmCategory ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MAX_OCCUPANTS", accommodation.maxOccupants);
                cmd.Parameters.AddWithValue("@AMENITIES", (object?)accommodation.amenities ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@GST_SLAB_PCT", accommodation.gst);
                cmd.Parameters.AddWithValue("@SAC_CODE", (object?)accommodation.sacCode ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IS_GST_INCLUSIVE", (object?)accommodation.isGstInclusive ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IS_FOOD_INCLUDED", (object?)accommodation.isFoodIncluded ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IS_AC", (object?)accommodation.isAc ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SORT_ORDER", accommodation.sortOrder);
                cmd.Parameters.AddWithValue("@REMARKS", (object?)accommodation.remarks ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ICON_CD", (object?)accommodation.iconCd ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ROOM_PRICE", accommodation.defaultRate);
                cmd.Parameters.AddWithValue("@STATUS", (object?)accommodation.status ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CREATE_BY", accommodation.userId);
                cmd.Parameters.AddWithValue("@MODIFY_BY", accommodation.userId);
                cmd.Parameters.AddWithValue("@GRP_ID", accommodation.grpId);
                cmd.Parameters.AddWithValue("@ORG_ID", accommodation.orgId);
                cmd.Parameters.AddWithValue("@LOC_ID", accommodation.locId);
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", accommodation.sessionId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while saving accommodation type.", ex);
            }
        }
        

        public List<AccomidationType> GetAccommodationTypes()
        {
            List<AccomidationType> list = new();

            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd = new SqlCommand("DGMS.PR_GET_ACCOMMODATION_TYPES", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            conn.Open();

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new AccomidationType
                {
                    accmTypeId = reader.SafeGetInt("DGM_ACCM_TYPE_MAST_ID"),
                    grpId = reader.SafeGetInt("GRP_ID"),
                    orgId = reader.SafeGetInt("ORG_ID"),
                    locId = reader.SafeGetInt("LOC_ID"),

                    accmTypeCode = reader.SafeGetString("ACCM_TYPE_CD"),
                    accmTypeName = reader.SafeGetString("ACCM_TYPE_NM"),
                    accmCategory = reader.SafeGetString("ACCM_CATEGORY"),

                    maxOccupants = reader.SafeGetInt("MAX_OCCUPANTS"),

                    amenities = reader.SafeGetString("AMENITIES"),

                    gst = reader.SafeGetDecimal("GST_SLAB_PCT") ?? 0,

                    sacCode = reader.SafeGetString("SAC_CODE"),

                    isGstInclusive = reader.SafeGetString("IS_GST_INCLUSIVE"),

                    isFoodIncluded = reader.SafeGetString("IS_FOOD_INCLUDED"),

                    isAc = reader.SafeGetString("IS_AC"),

                    sortOrder = reader.SafeGetInt("SORT_ORDER"),

                    remarks = reader.SafeGetString("REMARKS"),

                    iconCd = reader.SafeGetString("ICON_CD"),

                    status = reader.SafeGetString("RECORD_STATUS"),
                    defaultRate = reader.SafeGetDecimal("ROOM_PRICE") ?? 0,
                });
            }

            return list;
        }
    }
}