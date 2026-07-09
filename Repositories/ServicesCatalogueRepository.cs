using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Helpers;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class ServicesCatalogueRepository
    {
        private readonly string _conn;

        public ServicesCatalogueRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        public void SaveServiceCatalogue(ServiceCatalogue service)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);

                using SqlCommand cmd = new SqlCommand("DGMS.PR_INSUPD_SERVICE_CATALOGUE", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MODE", service.mode);

                cmd.Parameters.AddWithValue("@DGM_SERVICE_MAST_ID",
                    service.dgmServiceMastId == 0
                        ? DBNull.Value
                        : (object)service.dgmServiceMastId);

                cmd.Parameters.AddWithValue("@SERVICE_CD",
                    (object?)service.serviceCd ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@SERVICE_NM",
                    (object?)service.serviceNm ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@SERVICE_CATEGORY",
                    (object?)service.serviceCategory ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@PROPERTY_ID",
                    service.propertyId);

                cmd.Parameters.AddWithValue("@UNIT_OF_MEASURE",
                    (object?)service.unitOfMeasure ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@UNIT_RATE_AMT",
                    service.unitRateAmt);

                cmd.Parameters.AddWithValue("@GST_PCT",
                    service.gstPct);

                cmd.Parameters.AddWithValue("@SAC_CODE",
                    (object?)service.sacCode ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@IS_GST_INCLUSIVE",
                    (object?)service.isGstInclusive ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@IS_OPTIONAL",
                    (object?)service.isOptional ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@IS_AUTO_POST",
                    (object?)service.isAutoPost ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@DEPT_ID",
                    service.deptId);

                cmd.Parameters.AddWithValue("@COST_CENTER_CD",
                    (object?)service.costCenterCd ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@SORT_ORDER",
                    service.sortOrder);

                cmd.Parameters.AddWithValue("@REMARKS",
                    (object?)service.remarks ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@STATUS",
                    (object?)service.recordStatus ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@CREATE_BY", service.userId);
                cmd.Parameters.AddWithValue("@MODIFY_BY", service.userId);

                cmd.Parameters.AddWithValue("@GRP_ID", service.grpId);
                cmd.Parameters.AddWithValue("@ORG_ID", service.orgId);
                cmd.Parameters.AddWithValue("@LOC_ID", service.locId);

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", service.sessionId);

                conn.Open();

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while saving Service Catalogue.", ex);
            }
        }

        public List<ServiceCatalogue> GetServiceCatalogue()
        {
            List<ServiceCatalogue> list = new();

            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd = new SqlCommand("DGMS.PR_GET_SERVICE_CATALOGUE", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            conn.Open();

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new ServiceCatalogue
                {
                    dgmServiceMastId = reader.SafeGetInt("DGM_SERVICE_MAST_ID"),

                    grpId = reader.SafeGetInt("GRP_ID"),
                    orgId = reader.SafeGetInt("ORG_ID"),
                    locId = reader.SafeGetInt("LOC_ID"),

                    serviceCd = reader.SafeGetString("SERVICE_CD"),
                    serviceNm = reader.SafeGetString("SERVICE_NM"),
                    serviceCategory = reader.SafeGetString("SERVICE_CATEGORY"),

                    propertyId = reader.SafeGetInt("PROPERTY_ID"),

                    unitOfMeasure = reader.SafeGetString("UNIT_OF_MEASURE"),

                    unitRateAmt = (decimal)reader.SafeGetDecimal("UNIT_RATE_AMT"),

                    gstPct = (decimal)reader.SafeGetDecimal("GST_PCT"),

                    sacCode = reader.SafeGetString("SAC_CODE"),

                    isGstInclusive = reader.SafeGetString("IS_GST_INCLUSIVE"),

                    isOptional = reader.SafeGetString("IS_OPTIONAL"),

                    isAutoPost = reader.SafeGetString("IS_AUTO_POST"),

                    deptId = reader.SafeGetInt("DEPT_ID"),

                    costCenterCd = reader.SafeGetString("COST_CENTER_CD"),

                    sortOrder = (short)reader.SafeGetInt("SORT_ORDER"),

                    remarks = reader.SafeGetString("REMARKS"),

                    recordStatus = reader.SafeGetString("RECORD_STATUS")
                });
            }

            return list;
        }
    }
}