using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Helpers;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class OccupantGuestRepository
    {
        private readonly string _conn;

        public OccupantGuestRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        public void SaveOccupantGuest(OccupantGuest occupant)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);

                using SqlCommand cmd =
                    new SqlCommand("DGMS.PR_INSUPD_OCCUPANT_CATEGORY", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MODE", occupant.mode);

                cmd.Parameters.AddWithValue("@DGM_OCCUPANT_CAT_MAST_ID",
                    occupant.occupantCatId == 0
                        ? DBNull.Value
                        : (object)occupant.occupantCatId);

                cmd.Parameters.AddWithValue("@OCC_CAT_CD",
                    (object?)occupant.occupantCatCode ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@OCC_CAT_NM",
                    (object?)occupant.occupantCatName ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@BILLING_MODEL_CD",
                    (object?)occupant.billingModelCode ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@REQUIRE_PATIENT_LINK",
                    (object?)occupant.requirePatientLink ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@REQUIRE_ADMN_LINK",
                    (object?)occupant.requireAdmnLink ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@AUTO_CHECKOUT_ON_DISCH",
                    (object?)occupant.autoCheckoutOnDisch ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@ALLOW_CHARGE_TRANSFER",
                    (object?)occupant.allowChargeTransfer ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@IS_CORPORATE",
                    (object?)occupant.isCorporate ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@IS_NGO_SPONSORED",
                    (object?)occupant.isNgoSponsored ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@IS_COMPLIMENTARY",
                    (object?)occupant.isComplimentary ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@REQUIRE_ID_VERIFICATION",
                    (object?)occupant.requireIdVerification ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@MAX_STAY_DAYS",
                    (object?)occupant.maxStayDays ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@MIN_STAY_HOURS",
                    (object?)occupant.minStayHours ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@SECURITY_DEPOSIT_REQD",
                    (object?)occupant.securityDepositReqd ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@GST_APPLICABLE",
                    (object?)occupant.gstApplicable ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@VISITOR_PASS_REQD",
                    (object?)occupant.visitorPassReqd ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@SORT_ORDER",
                    occupant.sortOrder);

                cmd.Parameters.AddWithValue("@REMARKS",
                    (object?)occupant.remarks ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@STATUS",
                    (object?)occupant.status ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@CREATE_BY", occupant.userId);
                cmd.Parameters.AddWithValue("@MODIFY_BY", occupant.userId);

                cmd.Parameters.AddWithValue("@GRP_ID", occupant.grpId);
                cmd.Parameters.AddWithValue("@ORG_ID", occupant.orgId);
                cmd.Parameters.AddWithValue("@LOC_ID", occupant.locId);

                cmd.Parameters.AddWithValue("@IP_SESSION_ID",
                    occupant.sessionId);
                cmd.Parameters.AddWithValue("@IP_DISCOUNT",
                    occupant.discount);

                conn.Open();

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while saving Occupant Category.", ex);
            }
        }

        public List<OccupantGuest> GetOccupantGuest()
        {
            List<OccupantGuest> list = new();

            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd =
                new SqlCommand("DGMS.PR_GET_OCCUPANT_CATEGORY", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            conn.Open();

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new OccupantGuest
                {
                    occupantCatId = reader.SafeGetInt("DGM_OCCUPANT_CAT_MAST_ID"),

                    grpId = reader.SafeGetInt("GRP_ID"),
                    orgId = reader.SafeGetInt("ORG_ID"),
                    locId = reader.SafeGetInt("LOC_ID"),

                    occupantCatCode = reader.SafeGetString("OCC_CAT_CD"),
                    occupantCatName = reader.SafeGetString("OCC_CAT_NM"),

                    billingModelCode = reader.SafeGetString("BILLING_MODEL_CD"),

                    requirePatientLink = reader.SafeGetString("REQUIRE_PATIENT_LINK"),
                    requireAdmnLink = reader.SafeGetString("REQUIRE_ADMN_LINK"),
                    autoCheckoutOnDisch = reader.SafeGetString("AUTO_CHECKOUT_ON_DISCH"),
                    allowChargeTransfer = reader.SafeGetString("ALLOW_CHARGE_TRANSFER"),

                    isCorporate = reader.SafeGetString("IS_CORPORATE"),
                    isNgoSponsored = reader.SafeGetString("IS_NGO_SPONSORED"),
                    isComplimentary = reader.SafeGetString("IS_COMPLIMENTARY"),

                    requireIdVerification = reader.SafeGetString("REQUIRE_ID_VERIFICATION"),

                    maxStayDays = (short?)reader.SafeGetInt("MAX_STAY_DAYS"),
                    minStayHours = (short?)reader.SafeGetInt("MIN_STAY_HOURS"),

                    securityDepositReqd = reader.SafeGetString("SECURITY_DEPOSIT_REQD"),
                    gstApplicable = reader.SafeGetString("GST_APPLICABLE"),
                    visitorPassReqd = reader.SafeGetString("VISITOR_PASS_REQD"),

                    sortOrder = (short)reader.SafeGetInt("SORT_ORDER"),

                    remarks = reader.SafeGetString("REMARKS"),

                    status = reader.SafeGetString("RECORD_STATUS"),
                    discount = reader.SafeGetInt("DISCOUNT")
                });
            }

            return list;
        }
    }
}