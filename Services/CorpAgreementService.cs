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
            string msg = _repo.Update(_corp);

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
                    DiscountPct = row["DISCOUNT_PCT"] == DBNull.Value ? 0 : Convert.ToDecimal(row["DISCOUNT_PCT"]),
                    CreditDays = row["CREDIT_DAYS"] == DBNull.Value ? (short)0 : Convert.ToInt16(row["CREDIT_DAYS"]),
                    CreditLimitAmt = row["CREDIT_LIMIT_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(row["CREDIT_LIMIT_AMT"]),
                    EffectiveFromDt = Convert.ToDateTime(row["EFFECTIVE_FROM_DT"]),
                    EffectiveToDt = row["EFFECTIVE_TO_DT"] == DBNull.Value ? null : Convert.ToDateTime(row["EFFECTIVE_TO_DT"]),
                    RecordStatus = row["RECORD_STATUS"].ToString(),
                    CorpAddress = row["CORP_ADDRESS"].ToString(),
                    ContactNm = row["CONTACT_NM"].ToString(),
                    ContactPhone = row["CONTACT_PHONE"].ToString(),
                    propertyName = row["PROPERTY_NM"].ToString(),
                    roomAllocate = Convert.ToInt32(row["ROOM_ALLOCATE"] == DBNull.Value ? 0 : row["ROOM_ALLOCATE"]),
                    // NEW 02-Nov (2026-11-02 script); "ALL" before the script is run
                    corpBillingScope = Col(dt, row, "CORP_BILLING_SCOPE")?.ToString() ?? "ALL",

                    // NEW (05 script): the edit screen needs these, otherwise saving an edit wiped them
                    propertyId = Col(dt, row, "PROPERTY_ID") is object p ? Convert.ToInt32(p) : 0,
                    TariffId = Col(dt, row, "TARIFF_ID") is object tr ? Convert.ToInt32(tr) : 0,
                    ContactEmail = Col(dt, row, "CONTACT_EMAIL")?.ToString() ?? "",
                    IsGstRegistered = Col(dt, row, "IS_GST_REGISTERED")?.ToString() ?? "",
                    Remarks = Col(dt, row, "REMARKS")?.ToString()
                });
            }

            return ApiResponse<List<CorpAgreement>>.Success(
                "Corporate Agreement List",
                list);
        }

        /// <summary>Column value or null when the column is missing (05 not run yet) or NULL.</summary>
        private static object? Col(DataTable dt, DataRow row, string name)
        {
            if (!dt.Columns.Contains(name)) return null;
            return row[name] == DBNull.Value ? null : row[name];
        }


    }
}
