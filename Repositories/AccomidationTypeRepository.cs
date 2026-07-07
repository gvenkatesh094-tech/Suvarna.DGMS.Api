using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class AccomidationTypeRepository
    {
        private readonly string _connectionString;

        public AccomidationTypeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SqlServer");
        }

        public List<AccommodationType> GetAccommodationTypes()
        {
            List<AccommodationType> list = new List<AccommodationType>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DGMS.PR_GET_ACCOMMODATION_TYPES", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();

                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        list.Add(new AccommodationType
                        {
                            accmTypeId = Convert.ToInt32(dr["DGM_ACCM_TYPE_MAST_ID"]),
                            accmTypeCode = dr["ACCM_TYPE_CD"].ToString(),
                            accmTypeName = dr["ACCM_TYPE_NM"].ToString(),
                            gst = Convert.ToDecimal(dr["GST_SLAB_PCT"]),
                            roomPrice = Convert.ToDecimal(dr["ROOM_PRICE"])
                        });
                    }
                }
            }

            return list;
        }
    }
}