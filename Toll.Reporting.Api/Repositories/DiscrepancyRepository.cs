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
                    t,
                    s,
                    su,
                    l,
                    tt,
                    tc1,
                    tc2,
                    tc3,
                    tpd
                };

           
            if (operationalShift?.Any() == true)
            {
                query = query.Where(x => operationalShift.Contains(x.s != null ? x.s.Description : string.Empty));
            }

            if (tollOperators?.Any() == true)
            {
                query = query.Where(x => tollOperators.Contains(x.su != null ? x.su.Username : string.Empty));
            }

            if (laneNames?.Any() == true)
            {
                query = query.Where(x => laneNames.Contains(x.l != null ? x.l.LaneName : string.Empty));
            }

            if (paymentMethods?.Any() == true)
            {
                query = query.Where(x => paymentMethods.Contains(x.tt != null ? x.tt.Description : string.Empty));
            }

            var projected = query.Select(x => new DiscrepancyDto
            {
                Lane_Nr = x.t.LaneId,
                Trx_Sequence_Nr = x.t.TransactionNumber.ToString(),
                Trx_Date = x.t.TransactionDateTime.ToString("dd/MM/yyyy"),
                Trx_Time = x.t.TransactionDateTime.ToString("HH:mm:ss"),
                Operational_Shift = x.s != null ? x.s.Description : string.Empty,
                Toll_Operator_ID = x.su != null ? x.su.Username : string.Empty,
                Lane_Name = x.l != null ? x.l.LaneName : string.Empty,
                Method_of_Payment = x.tt != null ? x.tt.Description : string.Empty,
                Toll_Collector_Class = x.tc1 != null ? x.tc1.ClassDescription : string.Empty,
                AVC_Class = x.tc2 != null ? x.tc2.ClassDescription : string.Empty,
                Final_Class = x.tc3 != null ? x.tc3.ClassDescription : string.Empty,
                Tariff = x.tpd != null ? (decimal?)x.tpd.AmountInclusive : null,
                Updated_Tariff = x.tpd != null ? (decimal?)x.tpd.AmountExclusive : null,

                TakenAction = (x.tc1 != null ? x.tc1.ClassDescription : string.Empty) == (x.tc3 != null ? x.tc3.ClassDescription : string.Empty)
                    && (x.tc2 != null ? x.tc2.ClassDescription : string.Empty) == (x.tc3 != null ? x.tc3.ClassDescription : string.Empty)
                    ? "Both Correct"
                    : (x.tc1 != null ? x.tc1.ClassDescription : string.Empty) == (x.tc3 != null ? x.tc3.ClassDescription : string.Empty)
                        ? "Operator Correct"
                        : (x.tc2 != null ? x.tc2.ClassDescription : string.Empty) == (x.tc3 != null ? x.tc3.ClassDescription : string.Empty)
                            ? "AVC Correct"
                            : "Both Incorrect"
            });

            if (takenAction?.Any() == true)
            {
                projected = projected.Where(d => takenAction.Contains(d.TakenAction));
            }

            // Count and page
            var totalCount = await projected.CountAsync();

            var items = await projected
                .OrderBy(d => d.Trx_Date)
                .ThenBy(d => d.Trx_Time)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<DiscrepancyDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
