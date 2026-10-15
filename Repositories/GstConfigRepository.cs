using Suvarna.DGMS.Api.Helpers;
using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class GstConfigRepository
    {
        private readonly SessionContext _session;
        private readonly string _conn;

        public GstConfigRepository(IConfiguration configuration, SessionContext session)
        {
            _session = session;
            _conn = configuration.GetConnectionString("SqlServer");
        }

        #region SAVE

        public (bool Success, string Message, int NewId) Save(GstConfig gst)
        {
            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_IUD_DGM_GST_CONFIG", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IP_SESSION_ID", _session.Require());

            cmd.Parameters.AddWithValue("@IP_MODE", "INSERT");
            cmd.Parameters.AddWithValue("@IP_GRP_ID", gst.grpId);
            cmd.Parameters.AddWithValue("@IP_ORG_ID", gst.orgId);
            cmd.Parameters.AddWithValue("@IP_LOC_ID", gst.locId);
            cmd.Parameters.AddWithValue("@IP_PROPERTY_ID", gst.propertyId);
            cmd.Parameters.AddWithValue("@IP_SAC_CODE", gst.sacCode);
            cmd.Parameters.AddWithValue("@IP_SAC_DESCRIPTION", gst.sacDescription);
            cmd.Parameters.AddWithValue("@IP_GST_SLAB_CD", gst.gstSlabCd);
            cmd.Parameters.AddWithValue("@IP_GST_PCT", gst.gstPct);
            cmd.Parameters.AddWithValue("@IP_CGST_PCT", gst.cgstPct);
            cmd.Parameters.AddWithValue("@IP_SGST_PCT", gst.sgstPct);
            cmd.Parameters.AddWithValue("@IP_IGST_PCT", gst.igstPct);
            cmd.Parameters.AddWithValue("@IP_IS_INTRA_STATE", gst.isIntraState);
            cmd.Parameters.AddWithValue("@IP_STATE_CD", gst.stateCd);
            cmd.Parameters.AddWithValue("@IP_THRESHOLD_AMT_FROM", gst.thresholdAmtFrom);
            cmd.Parameters.AddWithValue("@IP_THRESHOLD_AMT_TO", gst.thresholdAmtTo);
            cmd.Parameters.AddWithValue("@IP_EFFECTIVE_FROM_DT", gst.effectiveFromDt);
            cmd.Parameters.AddWithValue("@IP_EFFECTIVE_TO_DT", (object)gst.effectiveToDt ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IP_REMARKS", "");
            cmd.Parameters.AddWithValue("@IP_RECORD_STATUS", gst.recordStatus);
            cmd.Parameters.AddWithValue("@IP_USER_ID", gst.createBy);

            var outId = new SqlParameter("@OUT_GST_CONFIG_ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
            var outMsg = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };

            cmd.Parameters.Add(outId);
            cmd.Parameters.Add(outStatus);
            cmd.Parameters.Add(outMsg);

            conn.Open();
            cmd.ExecuteNonQuery();

            bool success = outStatus.Value != DBNull.Value &&
                            outStatus.Value.ToString().Equals("Y", StringComparison.OrdinalIgnoreCase);

            string message = outMsg.Value == DBNull.Value ? string.Empty : outMsg.Value.ToString();
            int newId = outId.Value == DBNull.Value ? 0 : Convert.ToInt32(outId.Value);

            return (success, message, newId);
        }

        #endregion

        #region UPDATE

        public (bool Success, string Message) Update(GstConfig gst)
        {
            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_IUD_DGM_GST_CONFIG", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IP_SESSION_ID", _session.Require());

            cmd.Parameters.AddWithValue("@IP_MODE", "UPDATE");
            cmd.Parameters.AddWithValue("@IP_GST_CONFIG_ID", gst.gstConfigId);
            cmd.Parameters.AddWithValue("@IP_GRP_ID", gst.grpId);
            cmd.Parameters.AddWithValue("@IP_ORG_ID", gst.orgId);
            cmd.Parameters.AddWithValue("@IP_LOC_ID", gst.locId);
            cmd.Parameters.AddWithValue("@IP_PROPERTY_ID", gst.propertyId);
            cmd.Parameters.AddWithValue("@IP_SAC_CODE", gst.sacCode);
            cmd.Parameters.AddWithValue("@IP_SAC_DESCRIPTION", gst.sacDescription);
            cmd.Parameters.AddWithValue("@IP_GST_SLAB_CD", gst.gstSlabCd);
            cmd.Parameters.AddWithValue("@IP_GST_PCT", gst.gstPct);
            cmd.Parameters.AddWithValue("@IP_CGST_PCT", gst.cgstPct);
            cmd.Parameters.AddWithValue("@IP_SGST_PCT", gst.sgstPct);
            cmd.Parameters.AddWithValue("@IP_IGST_PCT", gst.igstPct);
            cmd.Parameters.AddWithValue("@IP_IS_INTRA_STATE", gst.isIntraState);
            cmd.Parameters.AddWithValue("@IP_STATE_CD", gst.stateCd);
            cmd.Parameters.AddWithValue("@IP_THRESHOLD_AMT_FROM", gst.thresholdAmtFrom);
            cmd.Parameters.AddWithValue("@IP_THRESHOLD_AMT_TO", gst.thresholdAmtTo);
            cmd.Parameters.AddWithValue("@IP_EFFECTIVE_FROM_DT", gst.effectiveFromDt);
            cmd.Parameters.AddWithValue("@IP_EFFECTIVE_TO_DT", (object)gst.effectiveToDt ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IP_REMARKS", "");// (object)gst.remarks ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IP_RECORD_STATUS", gst.recordStatus);
            // FIX: use updateBy, not createBy, on update
            cmd.Parameters.AddWithValue("@IP_USER_ID", gst.createBy);

            var outId = new SqlParameter("@OUT_GST_CONFIG_ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
            var outMsg = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };

            cmd.Parameters.Add(outId);
            cmd.Parameters.Add(outStatus);
            cmd.Parameters.Add(outMsg);

            conn.Open();
            cmd.ExecuteNonQuery();

            bool success = outStatus.Value != DBNull.Value &&
                outStatus.Value.ToString().Equals("Y", StringComparison.OrdinalIgnoreCase);

            string message = outMsg.Value == DBNull.Value ? string.Empty : outMsg.Value.ToString();

            return (success, message);
        }

        #endregion

        #region GET ALL

        public DataTable GetAll()
        {
            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_GET_DGM_GST_CONFIG", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IP_SESSION_ID", _session.Require());

            DataTable dt = new DataTable();
            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        #endregion

        #region GET BY ID

        public DataTable GetById(int gstConfigId)
        {
            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_GET_DGM_GST_CONFIG_BY_ID", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IP_SESSION_ID", _session.Require());
            cmd.Parameters.AddWithValue("@IP_GST_CONFIG_ID", gstConfigId);

            DataTable dt = new DataTable();
            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        #endregion
    }
}