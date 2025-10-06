using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace Toll.Reporting.Api.DTOs
{
    public class ComprehensiveDto
    {

        /*
         * L.LaneName,
		T.TransactionNumber,
		TT.Description AS TransactionType,
		DT.Description AS DiscountType,
		T.TransactionDateTime,
		S.Description AS Shift,
		T.ShiftDate,
		U.Username,
		TC.ClassDescription AS ManualTollClass,
		TP.TariffPlanId,
		TP.EffectiveDate,
		TP.CurrencyId,
		TPD.AmountInclusive
         * */

        //Start of Testing
        [BindNever]
        public string LaneName { get; set; }
        [BindNever]
        public long TransactionNumber { get; set; }
        [BindNever]
        public string TransactionType { get; set; }
        [BindNever]
        public string DiscountType { get; set; }
        [BindNever]
        public DateTime TransactionDateTime { get; set; }
        [BindNever]
        public string Shift { get; set; }
        [BindNever]
        public DateTime ShiftDate { get; set; }
        [BindNever]
        public string Username { get; set; }
        [BindNever]
        public string ManualTollClass { get; set; }
        [BindNever]
        public int TariffPlanId { get; set; }
        [BindNever]
        public DateTime EffectiveDate { get; set; }
        [BindNever]
        public int CurrencyId { get; set; }
        [BindNever]
        public double AmountInclusive { get; set; }
        [BindNever]
        //End of Testing
        public byte LaneId { get; set; }
        [BindNever]
        public byte TransactionTypeId { get; set; }
        [BindNever]
        public string MethodOfPayment { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        [BindNever]
        public byte DiscountTypeId { get; set; }
        [BindNever]
        public byte ShiftId { get; set; }
        [BindNever]
        public long? SystemUserId { get; set; }
        [BindNever]
        public byte ManualTollClassId { get; set; }
        [BindNever]
        public int TariffPlanDetailId { get; set; }
        public List<Dictionary<string, object>> TariffPlanDetails { get; set; } = new List<Dictionary<string, object>>();
    }
}
