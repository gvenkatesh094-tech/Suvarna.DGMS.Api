using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;
using System.Data;

namespace Suvarna.DGMS.Api.Services
{
    public class CorpAgreementService
    {
        private readonly CorpAgreementRepository _repo;

        public CorpAgreementService(CorpAgreementRepository repo)
        {
            _repo = repo;
        }
        public ApiResponse<string> Create(CorpAgreement _corp)
        {

            if (string.IsNullOrWhiteSpace(_corp.AgreementNo))
                return ApiResponse<string>.Fail("AgreementNo Number is required.");

            // Use the mode sent from Angular
            string msg = _repo.Save(_corp);

            return ApiResponse<string>.Success(msg, "OK");
        }

        public ApiResponse<string> Update(CorpAgreement _corp)
        {

            if (string.IsNullOrWhiteSpace(_corp.AgreementNo))
                return ApiResponse<string>.Fail("AgreementNo Number is required.");

            // Use the mode sent from Angular
            string msg = _repo.Save(_corp);

            return ApiResponse<string>.Success(msg, "OK");
        }
        public ApiResponse<List<CorpAgreement>> GetAll()
        {
            DataTable dt = _repo.GetAll();

            List<CorpAgreement> list = new List<CorpAgreement>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new CorpAgreement
                {
                    CorpAgreementMastId = Convert.ToInt32(row["DGM_CORP_AGREEMENT_MAST_ID"]),
                    AgreementNo = row["AGREEMENT_NO"].ToString(),
                    CorpNm = row["CORP_NM"].ToString(),
                    CorpGstin = row["CORP_GSTIN"].ToString(),
                    AgreementType = row["AGREEMENT_TYPE"].ToString(),
                    BillingCycle = row["BILLING_CYCLE"].ToString(),
                    DiscountPct = Convert.ToDecimal(row["DISCOUNT_PCT"]),
                    CreditDays = Convert.ToInt16(row["CREDIT_DAYS"]),
                    CreditLimitAmt = Convert.ToDecimal(row["CREDIT_LIMIT_AMT"]),
                    EffectiveFromDt = Convert.ToDateTime(row["EFFECTIVE_FROM_DT"]),
                    EffectiveToDt = Convert.ToDateTime(row["EFFECTIVE_TO_DT"]),
                    RecordStatus = row["RECORD_STATUS"].ToString()
                });
            }

            return ApiResponse<List<CorpAgreement>>.Success(
                "Corporate Agreement List",
                list);
        }


    }
}
