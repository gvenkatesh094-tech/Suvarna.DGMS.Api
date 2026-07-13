using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class CorpAgreementRepository
    {
        private readonly string _conn;

        public CorpAgreementRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }
        public string Save(CorpAgreement agreement)
        {
            return SaveAgreement(agreement, "I");
        }

        public string Update(CorpAgreement agreement)
        {
            return SaveAgreement(agreement, "U");
        }

        private string SaveAgreement(CorpAgreement agreement, string mode)
        {
            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd = new SqlCommand("DGMS.PR_IUD_CORP_AGREEMENT", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IP_SESSION_ID", agreement.sessionId);
            cmd.Parameters.AddWithValue("@IP_MODE", mode);
            if(mode=="U")
            {
                cmd.Parameters.AddWithValue("@IP_DGM_CORP_AGREEMENT_MAST_ID", agreement.CorpAgreementMastId);
            }
            else
            {
                cmd.Parameters.AddWithValue("@IP_DGM_CORP_AGREEMENT_MAST_ID", DBNull.Value);
            }
                
            cmd.Parameters.AddWithValue("@IP_GRP_ID", agreement.GrpId);
            cmd.Parameters.AddWithValue("@IP_ORG_ID", agreement.OrgId);
            cmd.Parameters.AddWithValue("@IP_LOC_ID", agreement.LocId);
            cmd.Parameters.AddWithValue("@IP_PROPERTY_ID", agreement.propertyId);            
            cmd.Parameters.AddWithValue("@IP_AGREEMENT_NO", agreement.AgreementNo);
            cmd.Parameters.AddWithValue("@IP_CORP_NM", agreement.CorpNm);
            cmd.Parameters.AddWithValue("@IP_CORP_GSTIN", agreement.CorpGstin);
            cmd.Parameters.AddWithValue("@IP_CORP_ADDRESS", agreement.CorpAddress);
            cmd.Parameters.AddWithValue("@IP_CONTACT_NM", agreement.ContactNm);
            cmd.Parameters.AddWithValue("@IP_CONTACT_EMAIL", agreement.ContactEmail);
            cmd.Parameters.AddWithValue("@IP_CONTACT_PHONE", agreement.ContactPhone);
            cmd.Parameters.AddWithValue("@IP_AGREEMENT_TYPE", agreement.AgreementType);
            cmd.Parameters.AddWithValue("@IP_BILLING_CYCLE", agreement.BillingCycle);
            cmd.Parameters.AddWithValue("@IP_CREDIT_LIMIT_AMT", agreement.CreditLimitAmt);
            cmd.Parameters.AddWithValue("@IP_CREDIT_DAYS", agreement.CreditDays);
            cmd.Parameters.AddWithValue("@IP_DISCOUNT_PCT", agreement.DiscountPct);
            cmd.Parameters.AddWithValue("@IP_TARIFF_ID", agreement.TariffId);
            cmd.Parameters.AddWithValue("@IP_IS_GST_REGISTERED", agreement.IsGstRegistered);
            cmd.Parameters.AddWithValue("@IP_EFFECTIVE_FROM_DT", agreement.EffectiveFromDt);
            cmd.Parameters.AddWithValue("@IP_EFFECTIVE_TO_DT", agreement.EffectiveToDt);
            cmd.Parameters.AddWithValue("@IP_REMARKS", agreement.Remarks);
            cmd.Parameters.AddWithValue("@IP_RECORD_STATUS", agreement.RecordStatus);

            SqlParameter outAgreementId = new SqlParameter("@OUT_AGREEMENT_ID", SqlDbType.Int);
            outAgreementId.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(outAgreementId);

            SqlParameter outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1);
            outStatus.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(outStatus);

            SqlParameter outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500);
            outMessage.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(outMessage);

            conn.Open();
            cmd.ExecuteNonQuery();

            return outMessage.Value.ToString();
        }
        #region Get All corps
        public DataTable GetAll()
        {
            using SqlConnection conn = new SqlConnection(_conn);

            using SqlCommand cmd = new SqlCommand("DGMS.PR_GET_DGM_CORP_AGREEMENT", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            DataTable dt = new DataTable();

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            da.Fill(dt);

            return dt;
        }

        #endregion
    }
}
