using Microsoft.Data.SqlClient;
using System.Data;
using Suvarna.DGMS.Api.Models;

namespace Suvarna.DGMS.Api.Repositories
{
    public interface IDepartmentMappingRepository
    {
        List<DepartmentCostMapping> GetAll();

        (bool Success, string Message, int NewId) Save(DepartmentCostMapping model);

        (bool Success, string Message) Update(DepartmentCostMapping model);

        (bool Success, string Message) Delete(int id, int userId);
    }

    public class DepartmentMappingRepository : IDepartmentMappingRepository
    {
        private readonly string _conn;

        public DepartmentMappingRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("DefaultConnection");
        }

        public List<DepartmentCostMapping> GetAll()
        {
            List<DepartmentCostMapping> list = new();

            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_GET_DGM_DEPT_MAPPING", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            conn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new DepartmentCostMapping
                {
                    deptMappingId = Convert.ToInt32(dr["DGM_DEPT_MAPPING_ID"]),
                    grpId = Convert.ToInt16(dr["GRP_ID"]),
                    orgId = Convert.ToInt16(dr["ORG_ID"]),
                    locId = Convert.ToInt16(dr["LOC_ID"]),
                    propertyId = Convert.ToInt32(dr["PROPERTY_ID"]),
                    accmTypeId = Convert.ToInt32(dr["ACCM_TYPE_ID"]),
                    occCatId = Convert.ToInt32(dr["OCC_CAT_ID"]),
                    deptId = Convert.ToInt32(dr["DEPT_ID"]),
                    deptCd = dr["DEPT_CD"].ToString(),
                    costCenterCd = dr["COST_CENTER_CD"].ToString(),
                    glAccountCd = dr["GL_ACCOUNT_CD"].ToString(),
                    remarks = dr["REMARKS"].ToString(),
                    recordStatus = dr["RECORD_STATUS"].ToString()
                });
            }

            return list;
        }

        public (bool Success, string Message, int NewId) Save(DepartmentCostMapping model)
        {
            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_IUD_DGM_DEPT_MAPPING", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IP_MODE", "INSERT");
            cmd.Parameters.AddWithValue("@IP_GRP_ID", model.grpId);
            cmd.Parameters.AddWithValue("@IP_ORG_ID", model.orgId);
            cmd.Parameters.AddWithValue("@IP_LOC_ID", model.locId);
            cmd.Parameters.AddWithValue("@IP_PROPERTY_ID", model.propertyId);
            cmd.Parameters.AddWithValue("@IP_ACCM_TYPE_ID", model.accmTypeId);
            cmd.Parameters.AddWithValue("@IP_OCC_CAT_ID", model.occCatId);
            cmd.Parameters.AddWithValue("@IP_DEPT_ID", model.deptId);
            cmd.Parameters.AddWithValue("@IP_DEPT_CD", model.deptCd);
            cmd.Parameters.AddWithValue("@IP_COST_CENTER_CD", model.costCenterCd);
            cmd.Parameters.AddWithValue("@IP_GL_ACCOUNT_CD", model.glAccountCd);
            cmd.Parameters.AddWithValue("@IP_REMARKS", (object?)model.remarks ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IP_RECORD_STATUS", model.recordStatus);
            cmd.Parameters.AddWithValue("@IP_USER_ID", model.createBy);

            SqlParameter outId = new("@OUT_DEPT_MAPPING_ID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            SqlParameter outStatus = new("@OUT_STATUS", SqlDbType.Char, 1)
            {
                Direction = ParameterDirection.Output
            };

            SqlParameter outMessage = new("@OUT_MESSAGE", SqlDbType.VarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            cmd.Parameters.Add(outId);
            cmd.Parameters.Add(outStatus);
            cmd.Parameters.Add(outMessage);

            conn.Open();
            cmd.ExecuteNonQuery();

            return
            (
                Convert.ToString(outStatus.Value) == "Y",
                Convert.ToString(outMessage.Value) ?? "",
                outId.Value == DBNull.Value ? 0 : Convert.ToInt32(outId.Value)
            );
        }

        public (bool Success, string Message) Update(DepartmentCostMapping model)
        {
            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_IUD_DGM_DEPT_MAPPING", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IP_MODE", "UPDATE");
            cmd.Parameters.AddWithValue("@IP_DEPT_MAPPING_ID", model.deptMappingId);
            cmd.Parameters.AddWithValue("@IP_GRP_ID", model.grpId);
            cmd.Parameters.AddWithValue("@IP_ORG_ID", model.orgId);
            cmd.Parameters.AddWithValue("@IP_LOC_ID", model.locId);
            cmd.Parameters.AddWithValue("@IP_PROPERTY_ID", model.propertyId);
            cmd.Parameters.AddWithValue("@IP_ACCM_TYPE_ID", model.accmTypeId);
            cmd.Parameters.AddWithValue("@IP_OCC_CAT_ID", model.occCatId);
            cmd.Parameters.AddWithValue("@IP_DEPT_ID", model.deptId);
            cmd.Parameters.AddWithValue("@IP_DEPT_CD", model.deptCd);
            cmd.Parameters.AddWithValue("@IP_COST_CENTER_CD", model.costCenterCd);
            cmd.Parameters.AddWithValue("@IP_GL_ACCOUNT_CD", model.glAccountCd);
            cmd.Parameters.AddWithValue("@IP_REMARKS", (object?)model.remarks ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IP_RECORD_STATUS", model.recordStatus);
            cmd.Parameters.AddWithValue("@IP_USER_ID", model.createBy);

            SqlParameter outId = new("@OUT_DEPT_MAPPING_ID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            SqlParameter outStatus = new("@OUT_STATUS", SqlDbType.Char, 1)
            {
                Direction = ParameterDirection.Output
            };

            SqlParameter outMessage = new("@OUT_MESSAGE", SqlDbType.VarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            cmd.Parameters.Add(outId);
            cmd.Parameters.Add(outStatus);
            cmd.Parameters.Add(outMessage);

            conn.Open();
            cmd.ExecuteNonQuery();

            return
            (
                Convert.ToString(outStatus.Value) == "Y",
                Convert.ToString(outMessage.Value) ?? ""
            );
        }

        public (bool Success, string Message) Delete(int id, int userId)
        {
            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_IUD_DGM_DEPT_MAPPING", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IP_MODE", "DELETE");
            cmd.Parameters.AddWithValue("@IP_DEPT_MAPPING_ID", id);
            cmd.Parameters.AddWithValue("@IP_USER_ID", userId);

            SqlParameter outId = new("@OUT_DEPT_MAPPING_ID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            SqlParameter outStatus = new("@OUT_STATUS", SqlDbType.Char, 1)
            {
                Direction = ParameterDirection.Output
            };

            SqlParameter outMessage = new("@OUT_MESSAGE", SqlDbType.VarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            cmd.Parameters.Add(outId);
            cmd.Parameters.Add(outStatus);
            cmd.Parameters.Add(outMessage);

            conn.Open();
            cmd.ExecuteNonQuery();

            return
            (
                Convert.ToString(outStatus.Value) == "Y",
                Convert.ToString(outMessage.Value) ?? ""
            );
        }
    }
}