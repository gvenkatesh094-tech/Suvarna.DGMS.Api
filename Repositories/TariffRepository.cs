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
            _conn = configuration.GetConnectionString("SqlServer");
        }

        // ==========================================
        // SAVE / UPDATE / DELETE
        // ==========================================

        public void SaveTariff(TariffSaveRequest tariff)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.pr_insupd_dgm_tariff", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MODE", tariff.mode);

                cmd.Parameters.AddWithValue("@DGM_TARIFF_MAST_ID",
                    tariff.tariffId == 0 ? DBNull.Value : tariff.tariffId);

                cmd.Parameters.AddWithValue("@TARIFF_CD", (object?)tariff.tariffCode ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@TARIFF_NM", (object?)tariff.tariffName ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@TARIFF_TYPE_CD", (object?)tariff.tariffType ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@PROPERTY_ID",
                    tariff.propertyId ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@ACCM_TYPE_ID",
                    tariff.accmTypeId ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@OCC_CAT_ID",
                    tariff.occCatId ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@BILLING_MODEL_ID",
                    tariff.billingModelId ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@BILLING_MODEL_CD",
                    (object?)tariff.billingModelCode ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@BILLING_MODEL_NAME",
                    (object?)tariff.billingModelName ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@DAILY_RATE_AMT", tariff.dailyRate);

                cmd.Parameters.AddWithValue("@HOURLY_RATE_AMT", tariff.hourlyRate);

                cmd.Parameters.AddWithValue("@MONTHLY_RATE_AMT", tariff.monthlyRate);

                cmd.Parameters.AddWithValue("@PACKAGE_RATE_AMT", tariff.packageRate);

                cmd.Parameters.AddWithValue("@PACKAGE_DAYS", tariff.packageDays);

                cmd.Parameters.AddWithValue("@MIN_BILLING_HOURS", tariff.minimumBillingHours);

                cmd.Parameters.AddWithValue("@CHECKIN_HOUR", tariff.checkinHour);

                cmd.Parameters.AddWithValue("@CHECKOUT_HOUR", tariff.checkoutHour);

                cmd.Parameters.AddWithValue("@GST_SLAB_PCT", tariff.gst);

                cmd.Parameters.AddWithValue("@IS_GST_INCLUSIVE",
                    tariff.isGstInclusive ?? "N");

                cmd.Parameters.AddWithValue("@SAC_CODE",
                    (object?)tariff.sacCode ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@CGST_PCT", tariff.cgst);

                cmd.Parameters.AddWithValue("@SGST_PCT", tariff.sgst);

                cmd.Parameters.AddWithValue("@IGST_PCT", tariff.igst);

                cmd.Parameters.AddWithValue("@IS_GST_EXEMPT",
                    tariff.isGstExempt ?? "N");

                cmd.Parameters.AddWithValue("@SECURITY_DEPOSIT_AMT",
                    tariff.deposit);

                cmd.Parameters.AddWithValue("@IS_DEPOSIT_REFUNDABLE",
                    tariff.isDepositRefundable ?? "Y");

                cmd.Parameters.AddWithValue("@EFFECTIVE_FROM_DT",
                    tariff.effectiveFrom ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@EFFECTIVE_TO_DT",
                    tariff.effectiveTo ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@IS_DYNAMIC_PRICING",
                    tariff.isDynamicPricing ?? "N");

                cmd.Parameters.AddWithValue("@PEAK_MULTIPLIER",
                    tariff.peakMultiplier);

                cmd.Parameters.AddWithValue("@OFF_PEAK_MULTIPLIER",
                    tariff.offPeakMultiplier);

                cmd.Parameters.AddWithValue("@IS_DEFAULT",
                    tariff.isDefault ?? "N");

                cmd.Parameters.AddWithValue("@REMARKS",
                    (object?)tariff.remarks ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@STATUS", tariff.status ?? "A");

                cmd.Parameters.AddWithValue("@CREATE_BY", tariff.userId);

                cmd.Parameters.AddWithValue("@MODIFY_BY", tariff.userId);

                cmd.Parameters.AddWithValue("@GRP_ID", tariff.grpId);

                cmd.Parameters.AddWithValue("@ORG_ID", tariff.orgId);

                cmd.Parameters.AddWithValue("@IP_LOCATION_ID", tariff.locationId);

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", tariff.sessionId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while saving tariff.", ex);
            }
        }

        // ==========================================
        // GET ALL
        // ==========================================

        public List<Tariff> GetAllTariffs(int sessionId, int locationId)
        {
            List<Tariff> list = new List<Tariff>();

            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.pr_get_dgm_tariff", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
            cmd.Parameters.AddWithValue("@IP_LOCATION_ID", locationId);

            conn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Tariff
                {
                    tariffId = Convert.ToInt32(dr["TariffId"]),
                    tariffCode = dr["TariffCode"]?.ToString(),
                    tariffName = dr["TariffName"]?.ToString(),
                    tariffType = dr["TariffType"]?.ToString(),

                    propertyId = dr["propertyId"] == DBNull.Value ? null : Convert.ToInt32(dr["propertyId"]),
                    billingModelId = dr["billingModelId"] == DBNull.Value ? null : Convert.ToInt32(dr["billingModelId"]),
                    accmTypeId = dr["AccmTypeId"] == DBNull.Value ? null : Convert.ToInt32(dr["AccmTypeId"]),
                    occCatId = dr["OccCatId"] == DBNull.Value ? null : Convert.ToInt32(dr["OccCatId"]),

                    billingModelCode = dr["billingModelCode"]?.ToString(),
                    billingModelName = dr["billingModelName"]?.ToString(),

                    rate = dr["Rate"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Rate"]),
                    gst = dr["GST"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["GST"]),
                    sacCode = dr["SacCode"]?.ToString(),
                    deposit = dr["Deposit"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Deposit"]),

                    status = dr["Status"]?.ToString(),

                    grpId = Convert.ToInt16(dr["GrpId"]),
                    orgId = Convert.ToInt16(dr["OrgId"]),
                    locationId = Convert.ToInt16(dr["LOCATION_ID"]),

                    dailyRate = dr["dailyRate"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["dailyRate"]),
                    hourlyRate = dr["hourlyRate"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["hourlyRate"]),
                    monthlyRate = dr["monthlyRate"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["monthlyRate"]),
                    packageRate = dr["packageRate"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["packageRate"]),

                    packageDays = dr["packageDays"] == DBNull.Value ? (short)0 : Convert.ToInt16(dr["packageDays"]),
                    minimumBillingHours = dr["minimumBillingHours"] == DBNull.Value ? (short)0 : Convert.ToInt16(dr["minimumBillingHours"]),

                    checkinHour = dr["checkinHour"] == DBNull.Value ? (short)0 : Convert.ToInt16(dr["checkinHour"]),
                    checkoutHour = dr["checkoutHour"] == DBNull.Value ? (short)0 : Convert.ToInt16(dr["checkoutHour"]),

                    isGstInclusive = dr["isGstInclusive"]?.ToString(),

                    cgst = dr["cgst"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["cgst"]),
                    sgst = dr["sgst"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["sgst"]),
                    igst = dr["igst"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["igst"]),

                    isGstExempt = dr["isGstExempt"]?.ToString(),
                    isDepositRefundable = dr["isDepositRefundable"]?.ToString(),

                    effectiveFrom = dr["effectiveFrom"] == DBNull.Value
        ? null
        : Convert.ToDateTime(dr["effectiveFrom"]),

                    effectiveTo = dr["effectiveTo"] == DBNull.Value
        ? null
        : Convert.ToDateTime(dr["effectiveTo"]),

                    isDynamicPricing = dr["isDynamicPricing"]?.ToString(),

                    peakMultiplier = dr["peakMultiplier"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["peakMultiplier"]),
                    offPeakMultiplier = dr["offPeakMultiplier"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["offPeakMultiplier"]),

                    isDefault = dr["isDefault"]?.ToString(),
                    remarks = dr["remarks"]?.ToString(),
                    accommodationName = dr["accommodationName"]?.ToString(),
                    propertyName = dr["propertyName"]?.ToString()
                });
            }

            return list;
        }
        public List<BillingModelMaster> GetAllBillingModels(int sessionId, int locationId)
        {
            List<BillingModelMaster> list = new List<BillingModelMaster>();

            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd = new SqlCommand("DGMS.PR_GET_BILLING_MODEL_MAST", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
            cmd.Parameters.AddWithValue("@IP_LOCATION_ID", locationId);

            conn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new BillingModelMaster
                {
                    billingModelId = Convert.ToInt32(dr["BILLING_MODEL_ID"]),
                    billingModelCode = dr["BILLING_MODEL_CODE"]?.ToString(),
                    billingModelName = dr["BILLING_MODEL_NAME"]?.ToString(),
                    description = dr["DESCRIPTION"]?.ToString(),
                    displayOrder = dr["DISPLAY_ORDER"] == DBNull.Value ? 0 : Convert.ToInt32(dr["DISPLAY_ORDER"]),
                    status = dr["STATUS"]?.ToString(),
                    createdBy = dr["CREATED_BY"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CREATED_BY"]),
                    createdDate = dr["CREATED_DATE"] == DBNull.Value ? null : Convert.ToDateTime(dr["CREATED_DATE"]),
                    modifiedBy = dr["MODIFIED_BY"] == DBNull.Value ? 0 : Convert.ToInt32(dr["MODIFIED_BY"]),
                    modifiedDate = dr["MODIFIED_DATE"] == DBNull.Value ? null : Convert.ToDateTime(dr["MODIFIED_DATE"])
                });
            }

            return list;
        }
    }
}