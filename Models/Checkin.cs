namespace Suvarna.DGMS.Api.Models
{
    public class Checkin
    {
 



        public int sessionId { get; set; } 

        public int? existingBookingId { get; set; }

        public int propertyId { get; set; }
        public int roomId { get; set; }
        public int? bedId { get; set; }
        public int tariffId { get; set; }
        public int occCatId { get; set; }

        public string guestName { get; set; } = "";
        public string? genderCd { get; set; }
        public DateTime? dob { get; set; }
        public string mobileNo { get; set; } = "";
        public string? emailId { get; set; }
        
        public string? occCatCd { get; set; }

        public int? genderId { get; set; }
        

        public int? idTypeId { get; set; }
        public string idTypeCd { get; set; } = "";

        public int? relationshipId { get; set; }
        public string? relationshipCd { get; set; }




        
        public string idNumberMasked { get; set; } = "";
        public string? photoPath { get; set; }

        public string? guestGstin { get; set; }
        public string? guestStateCd { get; set; }

        public string? vehicleNo { get; set; }
        public string? purpose { get; set; }

        public string? addressLine1 { get; set; }
        public string? city { get; set; }
        public string? stateCd { get; set; }

        public string? emergencyName { get; set; }
        public string? emergencyPhone { get; set; }



        public int? patientId { get; set; }
        public string? umrNo { get; set; }
        public int? admnId { get; set; }
        public string? admnNo { get; set; }
        

        public int? corpAgreementId { get; set; }
        public string? corpPoRef { get; set; }

        public DateTime expectedCheckoutDt { get; set; }

        // NEW: actual check-in time (null = now). Accepted by the proc for the last 24 hours only.
        public DateTime? checkinDt { get; set; }

        // NEW: 'Y' = staff confirmed the check-in although the company credit limit is crossed
        public string? creditOverride { get; set; }

        public decimal depositAmount { get; set; }
        public string? depositPaymentMode { get; set; }
        public string? depositRef { get; set; }
        public string? paymentMode { get; set; }
        public string? paymentRef { get; set; }
        public decimal totalPaidAmount { get; set; }


        public decimal baseAmount { get; set; }

        public decimal cgstAmount { get; set; }

        public decimal sgstAmount { get; set; }

                  
        public decimal igstAmount { get; set; }

        public decimal gstAmount { get; set; }

        public decimal grossAmount { get; set; }

        public decimal securityDepositAmount { get; set; }

        public decimal dailyRate { get; set; }

        public decimal hourlyRate { get; set; }

        public decimal monthlyRate { get; set; }

        public string billingModel { get; set; }
        public int userId { get; set; }

        // NEW: mode-specific payment capture (same as Checkin)
        public string? paymentRemarks { get; set; }
        public string? cardLast4 { get; set; }
        public string? upiVpa { get; set; }

        // NEW: SUCCESS POS payment (api/pos-payment) used for this check-in; RRN / card last 4 are taken from it
        public long? posTxnId { get; set; }
    }

    // NEW: result of PR_DGMS_GET_STAY_QUOTE - what check-in will charge for the stay
    public class StayQuote
    {
        public int tariffId { get; set; }
        public string billingModel { get; set; } = "";
        public int stayMinutes { get; set; }
        public int stayDays { get; set; }
        public int stayHours { get; set; }
        public decimal billedUnits { get; set; }
        public string unitLabel { get; set; } = "";
        public decimal dailyRate { get; set; }
        public decimal unitRate { get; set; }
        public decimal chargeAmount { get; set; }
        public decimal gstPct { get; set; }
        public decimal cgstPct { get; set; }
        public decimal sgstPct { get; set; }
        public decimal baseAmount { get; set; }
        public decimal cgstAmount { get; set; }
        public decimal sgstAmount { get; set; }
        public decimal igstAmount { get; set; }
        public decimal totalGst { get; set; }
        public decimal grossAmount { get; set; }
        public decimal securityDepositAmount { get; set; }

        // NEW: guest category rules applied to the room charge
        public decimal listChargeAmount { get; set; }
        public decimal discountPct { get; set; }
        public decimal discountAmount { get; set; }
        public string isComplimentary { get; set; } = "N";
    }

    public class CheckinResult
    {
        public int bookingId { get; set; }
        public string bookingNo { get; set; } = "";

        public int billId { get; set; }
        public string billNo { get; set; } = "";

        public string status { get; set; } = "";
        public string message { get; set; } = "";
    }
    public class PatientInfo
    {
        public int patientId { get; set; }
        public string gender { get; set; } = "";
        public string umrNo { get; set; } = "";

        public string admnNo { get; set; } = "";

        public string displayName { get; set; } = "";
    }
    public class PatientLookup
    {
        public int patientId { get; set; }
        public string umrNo { get; set; } = string.Empty;
        public string displayName { get; set; } = string.Empty;
        public string gender { get; set; } = string.Empty;
        public string admnNo { get; set; } = string.Empty;
        public string ipNo { get; set; } = string.Empty;

        public int? admnId { get; set; }
        public string? age { get; set; }
        public string? mobileNo { get; set; }
        public string? wardName { get; set; }
        public string? bedNo { get; set; }
        public DateTime? admnDate { get; set; }
        public string? dischargeStatus { get; set; }
    }
}