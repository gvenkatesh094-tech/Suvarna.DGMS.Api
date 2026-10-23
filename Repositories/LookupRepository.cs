using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class LookupRepository
    {
        private readonly string _conn;

        public LookupRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        public List<LookupModel> GetLookup(string lookupType)
        {
            List<LookupModel> list = new();

            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_LOOKUP", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IP_LOOKUP_TYPE", lookupType);

            conn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            // NEW: occupant category lookup also returns REQUIRE_PATIENT_LINK / REQUIRE_ADMN_LINK (04 script)
            var cols = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < dr.FieldCount; i++) cols[dr.GetName(i)] = i;

            string? Text(string name) =>
                cols.TryGetValue(name, out int i) && !dr.IsDBNull(i) ? Convert.ToString(dr.GetValue(i)) : null;
            decimal? Number(string name) =>
                cols.TryGetValue(name, out int i) && !dr.IsDBNull(i) ? Convert.ToDecimal(dr.GetValue(i)) : null;

            while (dr.Read())
            {
                list.Add(new LookupModel
                {
                    id = dr["ID"] == DBNull.Value
               ? 0
               : Convert.ToInt32(dr["ID"]),

                    cd = dr["CD"] == DBNull.Value
               ? string.Empty
               : Convert.ToString(dr["CD"]) ?? string.Empty,

                    name = dr["NAME"] == DBNull.Value
               ? string.Empty
               : Convert.ToString(dr["NAME"]) ?? string.Empty,

                    // occupant category rules (only the CATEGORY lookup returns them)
                    requirePatientLink = Text("REQUIRE_PATIENT_LINK"),
                    requireAdmnLink = Text("REQUIRE_ADMN_LINK"),
                    isCorporate = Text("IS_CORPORATE"),
                    requireIdVerification = Text("REQUIRE_ID_VERIFICATION"),
                    maxStayDays = (int?)Number("MAX_STAY_DAYS"),
                    isComplimentary = Text("IS_COMPLIMENTARY"),
                    securityDepositReqd = Text("SECURITY_DEPOSIT_REQD"),
                    discountPct = Number("DISCOUNT_PCT"),
                    gstApplicable = Text("GST_APPLICABLE")
                });
            }

            return list;
        }
    }
}