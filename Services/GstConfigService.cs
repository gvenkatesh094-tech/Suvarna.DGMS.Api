using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;
using System.Data;
using Suvarna.DGMS.Api.Helpers;

namespace Suvarna.DGMS.Api.Services
{
    public class GstConfigService
    {
        private readonly GstConfigRepository _repo;

        public GstConfigService(GstConfigRepository repo)
        {
            _repo = repo;
        }

        #region SAVE

        public ApiResponse<string> Save(GstConfig gst)
        {
            if (gst == null)
                return ApiResponse<string>.Fail("400", "Request body is empty.");

            // enforce CGST/SGST vs IGST business rule before hitting DB
            //gst.NormalizeGstSplit();

            try
            {
                var (success, message, newId) = _repo.Save(gst);

                if (!success)
                    return ApiResponse<string>.Fail("400", string.IsNullOrWhiteSpace(message)
                        ? "GST Configuration could not be saved."
                        : message);

                return ApiResponse<string>.Success(
                    string.IsNullOrWhiteSpace(message) ? "GST Configuration saved successfully." : message,
                    newId.ToString());
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Fail("500", $"Error saving GST Configuration: {ex.Message}");
            }
        }

        #endregion

        #region UPDATE

        public ApiResponse<string> Update(GstConfig gst)
        {
            if (gst == null || gst.gstConfigId <= 0)
                return ApiResponse<string>.Fail("400", "Invalid GST Configuration Id.");

         

            try
            {
                var (success, message) = _repo.Update(gst);

                if (!success)
                    return ApiResponse<string>.Fail("400", string.IsNullOrWhiteSpace(message)
                        ? "GST Configuration could not be updated."
                        : message);

                return ApiResponse<string>.Success(
                    string.IsNullOrWhiteSpace(message) ? "GST Configuration updated successfully." : message,
                    gst.gstConfigId.ToString());
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Fail("500", $"Error updating GST Configuration: {ex.Message}");
            }
        }

        #endregion

        #region GET ALL

        public ApiResponse<List<GstConfig>> GetAll()
        {
            try
            {
                DataTable dt = _repo.GetAll();
                List<GstConfig> list = new List<GstConfig>();

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new GstConfig
                    {
                        gstConfigId = Convert.ToInt32(row["DGM_GST_CONFIG_ID"]),
                        propertyId = Convert.ToInt32(row["PROPERTY_ID"]),
                        sacCode = row["SAC_CODE"] == DBNull.Value ? null : row["SAC_CODE"].ToString(),
                        sacDescription = row["SAC_DESCRIPTION"] == DBNull.Value ? null : row["SAC_DESCRIPTION"].ToString(),
                        gstSlabCd = row["GST_SLAB_CD"] == DBNull.Value ? null : row["GST_SLAB_CD"].ToString(),
                        gstPct = row["GST_PCT"] == DBNull.Value ? 0 : Convert.ToDecimal(row["GST_PCT"]),
                        cgstPct = row["CGST_PCT"] == DBNull.Value ? 0 : Convert.ToDecimal(row["CGST_PCT"]),
                        sgstPct = row["SGST_PCT"] == DBNull.Value ? 0 : Convert.ToDecimal(row["SGST_PCT"]),
                        igstPct = row["IGST_PCT"] == DBNull.Value ? 0 : Convert.ToDecimal(row["IGST_PCT"]),
                        isIntraState = row["IS_INTRA_STATE"] == DBNull.Value ? null : row["IS_INTRA_STATE"].ToString(),
                        stateCd = row["STATE_CD"] == DBNull.Value ? null : row["STATE_CD"].ToString(),
                        thresholdAmtFrom = row["THRESHOLD_AMT_FROM"] == DBNull.Value ? 0 : Convert.ToDecimal(row["THRESHOLD_AMT_FROM"]),
                        thresholdAmtTo = row["THRESHOLD_AMT_TO"] == DBNull.Value ? 0 : Convert.ToDecimal(row["THRESHOLD_AMT_TO"]),
                        effectiveFromDt = row["EFFECTIVE_FROM_DT"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["EFFECTIVE_FROM_DT"]),
                        effectiveToDt = row["EFFECTIVE_TO_DT"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["EFFECTIVE_TO_DT"]),
                        //remarks = "",
                        recordStatus = row["RECORD_STATUS"] == DBNull.Value ? null : row["RECORD_STATUS"].ToString()
                    });
                }

                return ApiResponse<List<GstConfig>>.Success("GST Configuration List", list);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<GstConfig>>.Fail("500", $"Error fetching GST Configurations: {ex.Message}");
            }
        }

        #endregion

        #region GET BY ID

        public ApiResponse<GstConfig> GetById(int gstConfigId)
        {
            if (gstConfigId <= 0)
                return ApiResponse<GstConfig>.Fail("400", "Invalid GST Configuration Id.");

            try
            {
                DataTable dt = _repo.GetById(gstConfigId);

                if (dt.Rows.Count == 0)
                    return ApiResponse<GstConfig>.Fail("404", "GST Configuration not found.");

                DataRow row = dt.Rows[0];

                GstConfig gst = new GstConfig
                {
                    gstConfigId = Convert.ToInt32(row["DGM_GST_CONFIG_ID"]),
                    grpId = Convert.ToInt16(row["GRP_ID"]),
                    orgId = Convert.ToInt16(row["ORG_ID"]),
                    locId = Convert.ToInt16(row["LOC_ID"]),
                    propertyId = Convert.ToInt32(row["PROPERTY_ID"]),
                    sacCode = row["SAC_CODE"] == DBNull.Value ? null : row["SAC_CODE"].ToString(),
                    sacDescription = row["SAC_DESCRIPTION"] == DBNull.Value ? null : row["SAC_DESCRIPTION"].ToString(),
                    gstSlabCd = row["GST_SLAB_CD"] == DBNull.Value ? null : row["GST_SLAB_CD"].ToString(),
                    gstPct = row["GST_PCT"] == DBNull.Value ? 0 : Convert.ToDecimal(row["GST_PCT"]),
                    cgstPct = row["CGST_PCT"] == DBNull.Value ? 0 : Convert.ToDecimal(row["CGST_PCT"]),
                    sgstPct = row["SGST_PCT"] == DBNull.Value ? 0 : Convert.ToDecimal(row["SGST_PCT"]),
                    igstPct = row["IGST_PCT"] == DBNull.Value ? 0 : Convert.ToDecimal(row["IGST_PCT"]),
                    isIntraState = row["IS_INTRA_STATE"] == DBNull.Value ? null : row["IS_INTRA_STATE"].ToString(),
                    stateCd = row["STATE_CD"] == DBNull.Value ? null : row["STATE_CD"].ToString(),
                    thresholdAmtFrom = row["THRESHOLD_AMT_FROM"] == DBNull.Value ? 0 : Convert.ToDecimal(row["THRESHOLD_AMT_FROM"]),
                    thresholdAmtTo = row["THRESHOLD_AMT_TO"] == DBNull.Value ? 0 : Convert.ToDecimal(row["THRESHOLD_AMT_TO"]),
                    effectiveFromDt = row["EFFECTIVE_FROM_DT"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["EFFECTIVE_FROM_DT"]),
                    effectiveToDt = row["EFFECTIVE_TO_DT"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["EFFECTIVE_TO_DT"]),
                   // remarks = "",//row["REMARKS"] == DBNull.Value ? null : row["REMARKS"].ToString(),
                    recordStatus = row["RECORD_STATUS"] == DBNull.Value ? null : row["RECORD_STATUS"].ToString(),
                    createBy = row["CREATE_BY"] == DBNull.Value ? 0 : Convert.ToInt32(row["CREATE_BY"]),
                    createDt = row["CREATE_DT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["CREATE_DT"])
                };

                return ApiResponse<GstConfig>.Success("GST Configuration", gst);
            }
            catch (Exception ex)
            {
                return ApiResponse<GstConfig>.Fail("500", $"Error fetching GST Configuration: {ex.Message}");
            }
        }

        #endregion
    }
}