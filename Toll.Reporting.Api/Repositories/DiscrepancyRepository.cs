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
            //  include full end-of-day if UI sends a date only
            endDate = NormalizeEndDate(endDate);

            //  normalize lists once (trim, remove empties, remove "-- All --")
            operationalShift = NormalizeFilterList(operationalShift);
            tollOperators = NormalizeFilterList(tollOperators);
            laneNames = NormalizeFilterList(laneNames);
            paymentMethods = NormalizeFilterList(paymentMethods);
            takenAction = NormalizeFilterList(takenAction);

            //  base query (SQL-side)
            var baseQuery =
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
                    } into tpdGroup
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

            //  BUSINESS RULE:
            // Hide when Toll Collector Class == AVC Class (not a discrepancy)
            baseQuery = baseQuery.Where(x =>
                (x.ManualClass ?? "").Trim() != (x.AutomaticClass ?? "").Trim()
            );

            //  apply filters only if user selected something
            if (operationalShift.Count > 0)
                baseQuery = baseQuery.Where(x => x.ShiftDescription != null && operationalShift.Contains(x.ShiftDescription));

            if (tollOperators.Count > 0)
                baseQuery = baseQuery.Where(x => x.OperatorUsername != null && tollOperators.Contains(x.OperatorUsername));

            if (laneNames.Count > 0)
                baseQuery = baseQuery.Where(x => x.LaneName != null && laneNames.Contains(x.LaneName));

            if (paymentMethods.Count > 0)
                baseQuery = baseQuery.Where(x => x.PaymentDescription != null && paymentMethods.Contains(x.PaymentDescription));

            // ✅ compute TakenAction in SQL-friendly way
            var withAction = baseQuery.Select(x => new
            {
                x,
                TakenAction =
                    string.IsNullOrEmpty(x.ActualClass) ? "" :
                    (x.ManualClass == x.ActualClass)
                        ? (x.AutomaticClass == x.ActualClass ? "" : "AVC Error")
                        : (x.AutomaticClass == x.ActualClass)
                            ? "Toll Collector Error"
                            : "Both Incorrect"
            });

            //  takenAction filter
            if (takenAction.Count > 0)
                withAction = withAction.Where(r => takenAction.Contains(r.TakenAction));

            var totalCount = await withAction.CountAsync();

            //  paging (exportAll passes pageSize=int.MaxValue)
            var ordered = withAction.OrderBy(r => r.x.TransactionDateTime);

            var pageQuery = (pageSize == int.MaxValue)
                ? ordered
                : ordered.Skip((page - 1) * pageSize).Take(pageSize);

            var data = await pageQuery.ToListAsync();

            var items = data.Select(r => new DiscrepancyDto
            {
                Lane_Nr = r.x.Lane_Nr,
                Trx_Sequence_Nr = r.x.Trx_Sequence_Nr.ToString(),
                Trx_Date = r.x.TransactionDateTime.ToString("dd/MM/yyyy"),
                Trx_Time = r.x.TransactionDateTime.ToString("HH:mm:ss"),

                Operational_Shift = r.x.ShiftDescription ?? "",
                Toll_Operator_ID = r.x.OperatorUsername ?? "",
                Lane_Name = r.x.LaneName ?? "",
                Method_of_Payment = r.x.PaymentDescription ?? "",

                Toll_Collector_Class = r.x.ManualClass ?? "",
                AVC_Class = r.x.AutomaticClass ?? "",
                Final_Class = r.x.ActualClass ?? "",

                Tariff = r.x.AmountInclusive ?? 0,
                Updated_Tariff = r.x.AmountExclusive ?? 0,

                TakenAction = r.TakenAction
            }).ToList();

            return new PagedResult<DiscrepancyDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (pageSize == int.MaxValue)
                    ? 1
                    : (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        // =============================
        // Helpers
        // =============================

        private static DateTime NormalizeEndDate(DateTime endDate)
        {
            return (endDate.TimeOfDay == TimeSpan.Zero)
                ? endDate.AddDays(1).AddSeconds(-1)
                : endDate;
        }

        private static List<string> NormalizeFilterList(List<string>? list)
        {
            return (list ?? new List<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Where(x => !x.Equals("-- All --", StringComparison.OrdinalIgnoreCase))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public async Task<DiscrepancyDto> GetDiscrepancyFilterOptionsAsync(DateTime startDate, DateTime endDate)
        {
            // ⚠️ IMPORTANT:
            // We are NOT filtering by date here.
            // We want ALL possible values in the system.

            var shifts = await _context.Shifts
                .AsNoTracking()
                .Select(s => s.Description)
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            var operators = await _context.SystemUsers
                .AsNoTracking()
                .Select(u => u.Username)
                .Where(u => !string.IsNullOrEmpty(u))
                .Distinct()
                .OrderBy(u => u)
                .ToListAsync();

            var lanes = await _context.Lanes
                .AsNoTracking()
                .Select(l => l.LaneName)
                .Where(l => !string.IsNullOrEmpty(l))
                .Distinct()
                .OrderBy(l => l)
                .ToListAsync();

            var paymentMethods = await _context.TransactionTypes
                .AsNoTracking()
                .Select(t => t.Description)
                .Where(t => !string.IsNullOrEmpty(t))
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            // TakenAction values are static logic-based values
            var takenActions = new List<string>
    {
        "AVC Error",
        "Toll Collector Error",
        "Both Incorrect"
    };

            return new DiscrepancyDto
            {
                Shifts = shifts,
                TollOperators = operators,
                Lanes = lanes,
                PaymentMethods = paymentMethods,
                TakenActions = takenActions
            };
        }
    }
}