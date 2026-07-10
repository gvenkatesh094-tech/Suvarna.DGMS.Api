using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Helpers;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class SystemParametersRepository
    {
        private readonly string _conn;

        public SystemParametersRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        public void SaveSystemParameter(SystemParameter parameter)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);

                using SqlCommand cmd = new SqlCommand(
                    "DGMS.PR_INSUPD_SYSTEM_PARAMETERS",
                    conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MODE", parameter.mode);

                cmd.Parameters.AddWithValue(
                    "@DGM_SYS_PARAM_ID",
                    parameter.dgmSysParamId == 0
                        ? DBNull.Value
                        : (object)parameter.dgmSysParamId);

                cmd.Parameters.AddWithValue(
                    "@PARAM_CD",
                    (object?)parameter.paramCd ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@PARAM_NM",
                    (object?)parameter.paramNm ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@PARAM_VALUE",
                    (object?)parameter.paramValue ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@PARAM_CATEGORY",
                    (object?)parameter.paramCategory ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@DATA_TYPE",
                    (object?)parameter.dataType ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@REMARKS",
                    (object?)parameter.remarks ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@STATUS",
                    (object?)parameter.recordStatus ?? DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@CREATE_BY",
                    parameter.userId);

                cmd.Parameters.AddWithValue(
                    "@MODIFY_BY",
                    parameter.userId);

                cmd.Parameters.AddWithValue(
                    "@GRP_ID",
                    parameter.grpId);

                cmd.Parameters.AddWithValue(
                    "@ORG_ID",
                    parameter.orgId);

                cmd.Parameters.AddWithValue(
                    "@LOC_ID",
                    parameter.locId);

                cmd.Parameters.AddWithValue(
                    "@IP_SESSION_ID",
                    parameter.sessionId);

                conn.Open();

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Error while saving System Parameter.",
                    ex);
            }
        }
        public List<SystemParameter> GetSystemParameters()
        {
            List<SystemParameter> list = new();

            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd = new SqlCommand(
                "DGMS.PR_GET_SYSTEM_PARAMETERS",
                conn);

            cmd.CommandType = CommandType.StoredProcedure;

            conn.Open();

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new SystemParameter
                {
                    dgmSysParamId = reader.SafeGetInt("DGM_SYS_PARAM_ID"),

                    grpId = reader.SafeGetInt("GRP_ID"),

                    orgId = reader.SafeGetInt("ORG_ID"),

                    locId = reader.SafeGetInt("LOC_ID"),

                    paramCd = reader.SafeGetString("PARAM_CD"),

                    paramNm = reader.SafeGetString("PARAM_NM"),

                    paramValue = reader.SafeGetString("PARAM_VALUE"),

                    paramCategory = reader.SafeGetString("PARAM_CATEGORY"),

                    dataType = reader.SafeGetString("DATA_TYPE"),

                    remarks = reader.SafeGetString("REMARKS"),

                    recordStatus = reader.SafeGetString("RECORD_STATUS"),

                    createBy = reader.SafeGetInt("CREATE_BY"),

                    createDt = reader.SafeGetDateTime("CREATE_DT"),

                    modifyBy = reader.SafeGetInt("MODIFY_BY"),

                    modifyDt = reader.SafeGetDateTime("MODIFY_DT")
                });
            }

            return list;
        }

        public List<string> GetCategories()
        {
            List<string> list = new();

            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd = new SqlCommand(
                "DGMS.PR_GET_SYSTEM_PARAMETER_CATEGORIES",
                conn);

            cmd.CommandType = CommandType.StoredProcedure;

            conn.Open();

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(
                    reader.SafeGetString("PARAM_CATEGORY")
                );
            }

            return list;
        }
    }
}