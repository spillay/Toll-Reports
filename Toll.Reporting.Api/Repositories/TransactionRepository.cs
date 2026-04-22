using Microsoft.EntityFrameworkCore;
using Toll.Reporting.Api.DTOs;
using TollReportingSystem.Data;

namespace Toll.Reporting.Api.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Fetch Transaction Details
        public async Task<PagedResult<TransactionDetailsDto>> GetTransactionDetailsAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            List<string>? laneNames = null,
            List<string>? paymentMethods = null,
            List<string>? tollCollectorClasses = null,
            int page = 1,
            int pageSize = 10)
        {
            // Include full end of day
            if (endDate.TimeOfDay == TimeSpan.Zero)
                endDate = endDate.AddDays(1).AddSeconds(-1);

            var query =
                from t in _context.Transactions.AsNoTracking()
                join s in _context.Shifts on t.ShiftId equals s.ShiftId into shiftGroup
                from s in shiftGroup.DefaultIfEmpty()
                join su in _context.SystemUsers on t.SystemUserId equals su.SystemUserId into userGroup
                from su in userGroup.DefaultIfEmpty()
                join l in _context.Lanes on t.LaneId equals l.LaneId into laneGroup
                from l in laneGroup.DefaultIfEmpty()
                join tt in _context.TransactionTypes on t.TransactionTypeId equals tt.TransactionTypeId into typeGroup
                from tt in typeGroup.DefaultIfEmpty()
                join tc1 in _context.TollClasses on t.ManualTollClassId equals tc1.TollClassId into tc1Group
                from tc1 in tc1Group.DefaultIfEmpty()
                join tc2 in _context.TollClasses on t.AutomaticTollClassId equals tc2.TollClassId into tc2Group
                from tc2 in tc2Group.DefaultIfEmpty()
                join tc3 in _context.TollClasses on t.ActualTollClassId equals tc3.TollClassId into tc3Group
                from tc3 in tc3Group.DefaultIfEmpty()
                    //join tpd in _context.TariffPlanDetails
                    //    on new { t.TariffPlanId, TollClassId = t.ManualTollClassId }
                    //    equals new { tpd.TariffPlanId, tpd.TollClassId } into tariffGroup
                    //from tpd in tariffGroup.DefaultIfEmpty()

                    // new (13-02-26) (duplication fix)

                join tpd in _context.TariffPlanDetails
                on new
                {
                    TariffPlanId = (int?)t.TariffPlanId,
                    TollClassId = (int?)t.ManualTollClassId,
                    TransactionTypeId = (int?)t.TransactionTypeId
                }
                equals new
                {
                    TariffPlanId = (int?)tpd.TariffPlanId,
                    TollClassId = (int?)tpd.TollClassId,
                    TransactionTypeId = (int?)tpd.TransactionTypeId
                } into tariffGroup
                            from tpd in tariffGroup.DefaultIfEmpty()



                    // end of duplication fix

                where t.TransactionDateTime >= startDate && t.TransactionDateTime <= endDate
                select new
                {
                    Transaction = t,
                    Shift = s,
                    Operator = su,
                    Lane = l,
                    Type = tt,
                    TollClass1 = tc1,
                    TollClass2 = tc2,
                    TollClass3 = tc3,
                    Tariff = tpd
                };

            //  Apply filters safely
            if (operationalShift?.Any() == true && !operationalShift.Contains("-- All --"))
                query = query.Where(x => x.Shift != null && operationalShift.Contains(x.Shift.Description));

            if (tollOperators?.Any() == true && !tollOperators.Contains("-- All --"))
                query = query.Where(x => x.Operator != null && tollOperators.Contains(x.Operator.Username));

            if (laneNames?.Any() == true && !laneNames.Contains("-- All --"))
                query = query.Where(x => x.Lane != null && laneNames.Contains(x.Lane.LaneName));

            if (paymentMethods?.Any() == true && !paymentMethods.Contains("-- All --"))
                query = query.Where(x => x.Type != null && paymentMethods.Contains(x.Type.Description));

            if (tollCollectorClasses?.Any() == true && !tollCollectorClasses.Contains("-- All --"))
                query = query.Where(x => x.TollClass1 != null && tollCollectorClasses.Contains(x.TollClass1.ClassDescription));

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.Transaction.TransactionDateTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new TransactionDetailsDto
                {
                    Lane_Nr = x.Transaction.LaneId,
                    Trx_Sequence_Nr = x.Transaction.TransactionNumber.ToString(),
                    Trx_Date = x.Transaction.TransactionDateTime.ToString("dd/MM/yyyy"),
                    Trx_Time = x.Transaction.TransactionDateTime.ToString("HH:mm:ss"),
                    Operational_Shift = x.Shift != null ? x.Shift.Description : "-- None --",
                    Toll_Operator_ID = x.Operator != null ? x.Operator.Username : "-- None --",
                    Lane_Name = x.Lane != null ? x.Lane.LaneName : "-- None --",
                    Method_of_Payment = x.Type != null ? x.Type.Description : "-- None --",
                    Toll_Collector_Class = x.TollClass1 != null ? x.TollClass1.ClassDescription : "-- None --",
                    AVC_Class = x.TollClass2 != null ? x.TollClass2.ClassDescription : "-- None --",
                    Final_Class = x.TollClass3 != null ? x.TollClass3.ClassDescription : "-- None --",
                    Tariff = x.Tariff != null ? (double?)x.Tariff.AmountInclusive : 0,
                    Tac_Card_Number = x.Transaction.CardNumber,
                    StartDate = startDate,
                    EndDate = endDate
                })
                .ToListAsync();

            return new PagedResult<TransactionDetailsDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        // 🔹 Fetch Filter Options
        public async Task<TransactionDetailsDto> GetTransactionFilterOptionsAsync(DateTime startDate, DateTime endDate)
        {
            var shifts = await _context.Shifts
                .Select(s => s.Description)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            var operators = await _context.SystemUsers
                .Select(u => u.Username)
                .Distinct()
                .OrderBy(u => u)
                .ToListAsync();

            var lanes = await _context.Lanes
                .Select(l => l.LaneName)
                .Distinct()
                .OrderBy(l => l)
                .ToListAsync();

            var paymentMethods = await _context.TransactionTypes // ✅ fixed (PaymentMethods table may not exist)
                .Select(pm => pm.Description)
                .Distinct()
                .OrderBy(pm => pm)
                .ToListAsync();

            var tollCollectorClasses = await _context.TollClasses
                .Where(tc => tc.ClassDescription != null && tc.ClassDescription != "")
                .Select(tc => tc.ClassDescription)
                .Distinct()
                .OrderBy(tc => tc)
                .ToListAsync();

            return new TransactionDetailsDto
            {
                StartDate = startDate,
                EndDate = endDate,
                Shifts = shifts,
                TollOperators = operators,
                Lanes = lanes,
                PaymentMethods = paymentMethods,
                TollCollectorClasses = tollCollectorClasses
            };
        }
    }
}
