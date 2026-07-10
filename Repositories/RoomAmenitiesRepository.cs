using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Helpers;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class RoomAmenitiesRepository
    {
        private readonly string _conn;

        public RoomAmenitiesRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        public void SaveRoomAmenity(RoomAmenity amenity)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);

                using SqlCommand cmd = new SqlCommand(
                    "DGMS.PR_INSUPD_ROOM_AMENITIES",
                    conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MODE", amenity.mode);

                cmd.Parameters.AddWithValue(
                    "@DGM_AMENITY_MAST_ID",
                    amenity.dgmAmenityMastId == 0
                        ? DBNull.Value
                        : (object)amenity.dgmAmenityMastId);

                cmd.Parameters.AddWithValue(
                    "@AMENITY_CD",
                    (object?)amenity.amenityCd ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@AMENITY_NM",
                    (object?)amenity.amenityNm ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@AMENITY_CATEGORY",
                    (object?)amenity.amenityCategory ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@ICON_CD",
                    (object?)amenity.iconCd ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@IS_CHARGEABLE",
                    (object?)amenity.isChargeable ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@CHARGE_AMT",
                    amenity.chargeAmt);

                cmd.Parameters.AddWithValue(
                    "@SORT_ORDER",
                    amenity.sortOrder);

                cmd.Parameters.AddWithValue(
                    "@STATUS",
                    (object?)amenity.recordStatus ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@CREATE_BY",
                    amenity.userId);

                cmd.Parameters.AddWithValue(
                    "@MODIFY_BY",
                    amenity.userId);

                cmd.Parameters.AddWithValue(
                    "@GRP_ID",
                    amenity.grpId);

                cmd.Parameters.AddWithValue(
                    "@ORG_ID",
                    amenity.orgId);

                cmd.Parameters.AddWithValue(
                    "@LOC_ID",
                    amenity.locId);

                cmd.Parameters.AddWithValue(
                    "@IP_SESSION_ID",
                    amenity.sessionId);
                cmd.Parameters.AddWithValue(
    "@IS_MANDATORY",
    (object?)amenity.isMandatory ?? DBNull.Value);

                conn.Open();

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Error while saving Room Amenity.",
                    ex);
            }
        }

        public List<RoomAmenity> GetRoomAmenities()
        {
            List<RoomAmenity> list = new();

            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd =
                new SqlCommand(
                    "DGMS.PR_GET_ROOM_AMENITIES",
                    conn);

            cmd.CommandType = CommandType.StoredProcedure;

            conn.Open();

            using SqlDataReader reader =
                cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new RoomAmenity
                {
                    dgmAmenityMastId =
                        reader.SafeGetInt("DGM_AMENITY_MAST_ID"),

                    grpId =
                        reader.SafeGetInt("GRP_ID"),

                    orgId =
                        reader.SafeGetInt("ORG_ID"),

                    locId =
                        reader.SafeGetInt("LOC_ID"),

                    amenityCd =
                        reader.SafeGetString("AMENITY_CD"),

                    amenityNm =
                        reader.SafeGetString("AMENITY_NM"),

                    amenityCategory =
                        reader.SafeGetString("AMENITY_CATEGORY"),

                    iconCd =
                        reader.SafeGetString("ICON_CD"),

                    isChargeable =
                        reader.SafeGetString("IS_CHARGEABLE"),

                    chargeAmt =
                        (decimal)reader.SafeGetDecimal("CHARGE_AMT"),

                    sortOrder =
                        (short)reader.SafeGetInt("SORT_ORDER"),

                    recordStatus =
                        reader.SafeGetString("RECORD_STATUS"),

                    createBy =
                        reader.SafeGetInt("CREATE_BY"),

                    createDt =
                        reader.SafeGetDateTime("CREATE_DT"),

                    modifyBy =
                        reader.SafeGetInt("MODIFY_BY"),

                    modifyDt =
                        reader.SafeGetDateTime("MODIFY_DT"),
                    isMandatory = reader.SafeGetString("IS_MANDATORY"),
                });
            }

            return list;
        }
        public List<string> GetAmenityCategories()
        {
            List<string> list = new();

            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd = new SqlCommand(
                "DGMS.PR_GET_AMENITY_CATEGORIES",
                conn);

            cmd.CommandType = CommandType.StoredProcedure;

            conn.Open();

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(reader.SafeGetString("AMENITY_CATEGORY"));
            }

            return list;
        }
    }
}