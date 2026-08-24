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

            SqlDataReader dr = cmd.ExecuteReader();

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
               : Convert.ToString(dr["NAME"]) ?? string.Empty
                });
            }

            return list;
        }
    }
}