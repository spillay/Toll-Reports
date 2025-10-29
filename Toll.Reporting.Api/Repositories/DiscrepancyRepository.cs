using Microsoft.EntityFrameworkCore;
using Toll.Reporting.Api.DTOs;
using TollReportingSystem.Data;

namespace Toll.Reporting.Api.Repositories
{
    public class DiscrepancyRepository : IDiscrepancyRepository
    {
        private readonly ApplicationDbContext _context;

        public DiscrepancyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<DiscrepancyDto>> GetDiscrepancyAsync(
             DateTime startDate,
             DateTime endDate,
             List<string>? operationalShift = null,
             List<string>? tollOperators = null,
             List<string>? laneNames = null,
             List<string>? paymentMethods = null,
             List<string>? takenAction = null,
             int page = 1,
             int pageSize = 50)
        {
            // Step 1. Build base query from database
            var query =
                from t in _context.Transactions
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
                join tpd in _context.TariffPlanDetails
                    on new { t.TariffPlanId, TollClassId = t.ManualTollClassId }
                    equals new { tpd.TariffPlanId, tpd.TollClassId } into tpdGroup
                from tpd in tpdGroup.DefaultIfEmpty()
                where t.TransactionDateTime >= startDate && t.TransactionDateTime <= endDate
                select new
                {
                    t.TransactionDateTime,
                    Lane_Nr = t.LaneId,
                    Trx_Sequence_Nr = t.TransactionNumber,
                    ShiftDescription = s.Description,
                    OperatorUsername = su.Username,
                    LaneName = l.LaneName,
                    PaymentDescription = tt.Description,
                    ManualClass = tc1.ClassDescription,
                    AutomaticClass = tc2.ClassDescription,
                    ActualClass = tc3.ClassDescription,
                    AmountInclusive = (decimal?)tpd.AmountInclusive,
                    AmountExclusive = (decimal?)tpd.AmountExclusive
                };

            // Step 2. Apply pre-projection filters
            if (operationalShift?.Any() == true)
                query = query.Where(x => operationalShift.Contains(x.ShiftDescription));

            if (tollOperators?.Any() == true)
                query = query.Where(x => tollOperators.Contains(x.OperatorUsername));

            if (laneNames?.Any() == true)
                query = query.Where(x => laneNames.Contains(x.LaneName));

            if (paymentMethods?.Any() == true)
                query = query.Where(x => paymentMethods.Contains(x.PaymentDescription));

            // Step 3. Fetch data (so we can use .ToString formatting safely)
            var rawData = await query.ToListAsync();

            // Step 4. Project to DTOs (in memory)
            var projected = rawData.Select(x => new DiscrepancyDto
            {
                Lane_Nr = x.Lane_Nr,
                Trx_Sequence_Nr = x.Trx_Sequence_Nr.ToString(),
                Trx_Date = x.TransactionDateTime.ToString("dd/MM/yyyy"),
                Trx_Time = x.TransactionDateTime.ToString("HH:mm:ss"),
                Operational_Shift = x.ShiftDescription ?? "",
                Toll_Operator_ID = x.OperatorUsername ?? "",
                Lane_Name = x.LaneName ?? "",
                Method_of_Payment = x.PaymentDescription ?? "",
                Toll_Collector_Class = x.ManualClass ?? "",
                AVC_Class = x.AutomaticClass ?? "",
                Final_Class = x.ActualClass ?? "",
                Tariff = x.AmountInclusive ?? 0,
                Updated_Tariff = x.AmountExclusive ?? 0,
                TakenAction =
                    (x.ManualClass == x.ActualClass && x.AutomaticClass == x.ActualClass)
                        ? "Both Correct"
                        : (x.ManualClass == x.ActualClass)
                            ? "Operator Correct"
                            : (x.AutomaticClass == x.ActualClass)
                                ? "AVC Correct"
                                : "Both Incorrect"
            }).ToList();

            // ✅ Step 5. Apply TakenAction filter (case-insensitive)
            if (takenAction?.Any() == true)
            {
                var normalizedActions = takenAction
                    .Where(a => !string.IsNullOrWhiteSpace(a))
                    .Select(a => a.Trim().ToLower())
                    .ToList();

                projected = projected
                    .Where(d => normalizedActions.Contains(d.TakenAction.Trim().ToLower()))
                    .ToList();
            }

            // Step 6. Pagination
            var totalCount = projected.Count;
            var pagedItems = projected
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Step 7. Return paged result
            return new PagedResult<DiscrepancyDto>
            {
                Items = pagedItems,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
    }
}
