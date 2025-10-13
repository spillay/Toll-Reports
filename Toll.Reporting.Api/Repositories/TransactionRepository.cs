using Microsoft.EntityFrameworkCore;
using Toll.Reporting.Api.DTOs;
using TollReportingSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toll.Reporting.Api.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Fetch paginated transaction details with optional filters.
        /// </summary>
        public async Task<PagedResult<TransactionDetailsDto>> GetTransactionDetailsAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            List<string>? laneNames = null,
            List<string>? paymentMethods = null,
            int page = 1,
            int pageSize = 10)
        {
            // Base query joining related tables
            var query = from t in _context.Transactions

                        join s in _context.Shifts on t.ShiftId equals s.ShiftId into shiftGroup
                        from s in shiftGroup.DefaultIfEmpty()

                        join su in _context.SystemUsers on t.SystemUserId equals su.SystemUserId into userGroup
                        from su in userGroup.DefaultIfEmpty()

                        join l in _context.Lanes on t.LaneId equals l.LaneId into laneGroup
                        from l in laneGroup.DefaultIfEmpty()

                        join tt in _context.TransactionTypes
                            on t.TransactionTypeId equals tt.TransactionTypeId into typeGroup
                        from tt in typeGroup.DefaultIfEmpty()

                        join tc1 in _context.TollClasses on t.ManualTollClassId equals tc1.TollClassId into tc1Group
                        from tc1 in tc1Group.DefaultIfEmpty()

                        join tc2 in _context.TollClasses on t.AutomaticTollClassId equals tc2.TollClassId into tc2Group
                        from tc2 in tc2Group.DefaultIfEmpty()

                        join tc3 in _context.TollClasses on t.ActualTollClassId equals tc3.TollClassId into tc3Group
                        from tc3 in tc3Group.DefaultIfEmpty()

                        join tpd in _context.TariffPlanDetails
                            on new { t.TariffPlanId, TollClassId = t.ManualTollClassId }
                            equals new { tpd.TariffPlanId, tpd.TollClassId } into tariffGroup
                        from tpd in tariffGroup.DefaultIfEmpty()

                        where t.TransactionDateTime >= startDate &&
                              t.TransactionDateTime < endDate.AddDays(1)
                        orderby t.TransactionDateTime descending

                        select new
                        {
                            Transaction = t,
                            Shift = s,
                            User = su,
                            Lane = l,
                            Type = tt,
                            TollClass1 = tc1,
                            TollClass2 = tc2,
                            TollClass3 = tc3,
                            Tariff = tpd
                        };

            // ✅ Apply optional filters dynamically
            if (operationalShift != null && operationalShift.Any() && !operationalShift.Contains("-- All --"))
            {
                query = query.Where(x => operationalShift.Contains(x.Shift.Description));
            }

            if (tollOperators != null && tollOperators.Any() && !tollOperators.Contains("-- All --"))
            {
                query = query.Where(x => tollOperators.Contains(x.User.Username));
            }

            if (laneNames != null && laneNames.Any() && !laneNames.Contains("-- All --"))
            {
                query = query.Where(x => laneNames.Contains(x.Lane.LaneName));
            }

            if (paymentMethods != null && paymentMethods.Any() && !paymentMethods.Contains("-- All --"))
            {
                query = query.Where(x => paymentMethods.Contains(x.Type.Description));
            }

            // Count total records before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var pagedItems = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new TransactionDetailsDto
                {
                    Lane_Nr = x.Transaction.LaneId,
                    Trx_Sequence_Nr = x.Transaction.TransactionNumber.ToString(),
                    Trx_Date = x.Transaction.TransactionDateTime.ToString("dd/MM/yyyy"),
                    Trx_Time = x.Transaction.TransactionDateTime.ToString("HH:mm:ss"),
                    Operational_Shift = x.Shift.Description ?? "-- None --",
                    Toll_Operator_ID = x.User.Username ?? "-- None --",
                    Lane_Name = x.Lane.LaneName ?? "-- None --",
                    Method_of_Payment = x.Type.Description ?? "-- None --",
                    Toll_Collector_Class = x.TollClass1.ClassDescription ?? "-- None --",
                    AVC_Class = x.TollClass2.ClassDescription ?? "-- None --",
                    Final_Class = x.TollClass3.ClassDescription ?? "-- None --",
                    Tariff = Convert.ToDecimal(x.Tariff != null ? x.Tariff.AmountInclusive : 0),
                    Tac_Card_Number = x.Transaction.CardNumber,
                    StartDate = startDate,
                    EndDate = endDate
                })
                .ToListAsync();

            //  Return paged result
            return new PagedResult<TransactionDetailsDto>
            {
                Items = pagedItems,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

        }

        // ==================== LOOKUP QUERIES ====================

        public async Task<IEnumerable<string>> GetShiftsAsync()
        {
            return await _context.Shifts
                                 .Select(s => s.Description)
                                 .Distinct()
                                 .OrderBy(s => s)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetTollOperatorsAsync()
        {
            return await _context.SystemUsers
                                 .Select(su => su.Username)
                                 .Distinct()
                                 .OrderBy(su => su)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetLanesAsync()
        {
            return await _context.Lanes
                                 .Select(l => l.LaneName)
                                 .Distinct()
                                 .OrderBy(l => l)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetPaymentMethodsAsync()
        {
            return await _context.PaymentMethods
                                 .Select(pm => pm.Description)
                                 .Distinct()
                                 .OrderBy(pm => pm)
                                 .ToListAsync();
        }
    }
}
