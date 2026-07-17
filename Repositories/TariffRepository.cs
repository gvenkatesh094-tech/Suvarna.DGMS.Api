using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class TariffRepository
    {
        private readonly string _conn;

        public TariffRepository(
            IConfiguration configuration)
        {
            _conn = configuration
                .GetConnectionString("SqlServer");
        }

        // ==========================================
        // GENERATE CODE
        // ==========================================

        public string GenerateCodeTariff(
            GenerateCodeRequest request)
        {
            try
            {
                using SqlConnection conn =
                    new SqlConnection(_conn);

                using SqlCommand cmd =
                    new SqlCommand(
                        "PR_GET_PAGELOAD_TABKEY",
                        conn
                    );

                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@IP_TABLE_NAME",
                    request.masterName
                );

                cmd.Parameters.AddWithValue(
                    "@IP_SESSION_ID",
                    request.sessionId
                );

                SqlParameter code =
                    new SqlParameter(
                        "@OP_TABKEY_CD",
                        SqlDbType.VarChar,
                        20
                    );

                code.Direction =
                    ParameterDirection.Output;

                cmd.Parameters.Add(code);

                conn.Open();

                cmd.ExecuteNonQuery();

                return Convert.ToString(
                    code.Value
                ) ?? "";
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Error generating Tariff Code.",
                    ex
                );
            }
        }

        // ==========================================
        // SAVE / UPDATE / DELETE
        // ==========================================

        public void SaveTariff(
            Tariff tariff)
        {
            try
            {
                using SqlConnection conn =
                    new SqlConnection(_conn);

                using SqlCommand cmd =
                    new SqlCommand(
                        "DGMS.pr_insupd_dgm_tariff",
                        conn
                    );

                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@MODE",
                    tariff.mode
                );

                cmd.Parameters.AddWithValue(
                    "@DGM_TARIFF_MAST_ID",
                    tariff.tariffId == 0
                        ? DBNull.Value
                        : (object)tariff.tariffId
                );

                cmd.Parameters.AddWithValue(
                    "@TARIFF_CD",
                    (object?)tariff.tariffCode
                    ?? DBNull.Value
                );

                cmd.Parameters.AddWithValue(
                    "@TARIFF_NM",
                    (object?)tariff.tariffName
                    ?? DBNull.Value
                );

                cmd.Parameters.AddWithValue(
                    "@TARIFF_TYPE_CD",
                    (object?)tariff.tariffType
                    ?? DBNull.Value
                );

                cmd.Parameters.AddWithValue(
                    "@PROPERTY_ID",
                    tariff.propertyId.HasValue
                        ? tariff.propertyId.Value
                        : DBNull.Value
                );

                cmd.Parameters.AddWithValue(
                    "@ACCM_TYPE_ID",
                    tariff.accmTypeId.HasValue
                        ? tariff.accmTypeId.Value
                        : DBNull.Value
                );

                cmd.Parameters.AddWithValue(
                    "@OCC_CAT_ID",
                    tariff.occCatId.HasValue
                        ? tariff.occCatId.Value
                        : DBNull.Value
                );

                cmd.Parameters.AddWithValue(
                    "@BILLING_MODEL_CD",
                    (object?)tariff.billingModel
                    ?? DBNull.Value
                );

                cmd.Parameters.AddWithValue(
                    "@RATE",
                    tariff.rate
                );

                cmd.Parameters.AddWithValue(
                    "@GST_SLAB_PCT",
                    tariff.gst
                );

                cmd.Parameters.AddWithValue(
                    "@SAC_CODE",
                    (object?)tariff.sacCode
                    ?? DBNull.Value
                );

                cmd.Parameters.AddWithValue(
                    "@SECURITY_DEPOSIT_AMT",
                    tariff.deposit
                );

                cmd.Parameters.AddWithValue(
                    "@STATUS",
                    (object?)tariff.status
                    ?? DBNull.Value
                );

                cmd.Parameters.AddWithValue(
                    "@CREATE_BY",
                    tariff.userId
                );

                cmd.Parameters.AddWithValue(
                    "@MODIFY_BY",
                    tariff.userId
                );

                cmd.Parameters.AddWithValue(
                    "@GRP_ID",
                    tariff.grpId
                );

                cmd.Parameters.AddWithValue(
                    "@ORG_ID",
                    tariff.orgId
                );

                cmd.Parameters.AddWithValue(
                    "@LOC_ID",
                    tariff.locId
                );

                cmd.Parameters.AddWithValue(
                    "@IP_SESSION_ID",
                    tariff.sessionId
                );

                conn.Open();

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Error while saving tariff.",
                    ex
                );
            }
        }

        // ==========================================
        // GET ALL
        // ==========================================

        public List<Tariff> GetAllTariffs()
        {
            List<Tariff> list =
                new List<Tariff>();

            using SqlConnection conn =
                new SqlConnection(_conn);

            using SqlCommand cmd =
                new SqlCommand(
                    "DGMS.pr_get_dgm_tariff",
                    conn
                );

            cmd.CommandType =
                CommandType.StoredProcedure;

            conn.Open();

            using SqlDataReader dr =
                cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(
                    new Tariff
                    {
                        tariffId =
                            Convert.ToInt32(
                                dr["TariffId"]
                            ),

                        tariffCode =
                            dr["TariffCode"]
                            ?.ToString(),

                        tariffName =
                            dr["TariffName"]
                            ?.ToString(),

                        tariffType =
                            dr["TariffType"]
                            ?.ToString(),

                        accmTypeId =
                            dr["AccmTypeId"] == DBNull.Value
                                ? null
                                : Convert.ToInt32(
                                    dr["AccmTypeId"]
                                ),

                        billingModel =
                            dr["BillingModel"]
                            ?.ToString(),

                        rate =
                            dr["Rate"] == DBNull.Value
                                ? 0
                                : Convert.ToDecimal(
                                    dr["Rate"]
                                ),

                        gst =
                            dr["GST"] == DBNull.Value
                                ? 0
                                : Convert.ToDecimal(
                                    dr["GST"]
                                ),

                        sacCode =
                            dr["SacCode"]
                            ?.ToString(),

                        deposit =
                            dr["Deposit"] == DBNull.Value
                                ? 0
                                : Convert.ToDecimal(
                                    dr["Deposit"]
                                ),

                        status =
                            dr["Status"]
                            ?.ToString(),

                        grpId =
                            Convert.ToInt16(
                                dr["GrpId"]
                            ),

                        orgId =
                            Convert.ToInt16(
                                dr["OrgId"]
                            ),

                        locId =
                            Convert.ToInt16(
                                dr["LocId"]
                            )
                    }
                );
            }

            return list;
        }
    }
}