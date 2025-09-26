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

        public async Task<IEnumerable<TransactionDetailsDto>> GetTransactionDetailsAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            List<string>? laneNames = null,
            List<string>? paymentMethods = null)
        {
            // Query Transactions with LEFT JOIN equivalents
            var query = from t in _context.Transactions

                            // LEFT JOIN Shift
                        join s in _context.Shifts on t.ShiftId equals s.ShiftId into shiftGroup
                        from s in shiftGroup.DefaultIfEmpty()

                            // LEFT JOIN SystemUser
                        join su in _context.SystemUsers on t.SystemUserId equals su.SystemUserId into userGroup
                        from su in userGroup.DefaultIfEmpty()

                            // LEFT JOIN Lane
                        join l in _context.Lanes on t.LaneId equals l.LaneId into laneGroup
                        from l in laneGroup.DefaultIfEmpty()

                            // LEFT JOIN PaymentMethod
                        join pm in _context.PaymentMethods on t.TransactionTypeId equals pm.PaymentMethodId into paymentGroup
                        from pm in paymentGroup.DefaultIfEmpty()

                            // LEFT JOIN TollClass (Manual, Automatic, Actual)
                        join tc1 in _context.TollClasses on t.ManualTollClassId equals tc1.TollClassId into tc1Group
                        from tc1 in tc1Group.DefaultIfEmpty()

                        join tc2 in _context.TollClasses on t.AutomaticTollClassId equals tc2.TollClassId into tc2Group
                        from tc2 in tc2Group.DefaultIfEmpty()

                        join tc3 in _context.TollClasses on t.ActualTollClassId equals tc3.TollClassId into tc3Group
                        from tc3 in tc3Group.DefaultIfEmpty()

                            // LEFT JOIN TariffPlanDetail
                        join tpd in _context.TariffPlanDetails
                             on new { t.TariffPlanId, TollClassId = t.ManualTollClassId }
                             equals new { tpd.TariffPlanId, tpd.TollClassId } into tariffGroup
                        from tpd in tariffGroup.DefaultIfEmpty()

                        where t.TransactionDateTime >= startDate &&
                              t.TransactionDateTime < endDate.AddDays(1)

                        // Apply filters like in the SQL
                        where (operationalShift == null || operationalShift.Contains("-- All --") || operationalShift.Contains(s.Description))
                           && (tollOperators == null || tollOperators.Contains("-- All --") || tollOperators.Contains(su.Username))
                           && (laneNames == null || laneNames.Contains("-- All --") || laneNames.Contains(l.LaneName))
                           && (paymentMethods == null || paymentMethods.Contains("-- All --") || paymentMethods.Contains(pm.Description))

                        orderby t.TransactionDateTime descending

                        select new TransactionDetailsDto
                        {
                            Lane_Nr = t.LaneId,
                            //Trx_Sequence_Nr = t.TransactionNumber,
                            Trx_Sequence_Nr = t.TransactionNumber.ToString(),
                            Trx_Date = t.TransactionDateTime.ToString("dd/MM/yyyy"),
                            Trx_Time = t.TransactionDateTime.ToString("HH:mm:ss:fff"),
                            Operational_Shift = s.Description ?? "-- None --",
                            Toll_Operator_ID = su.Username ?? "-- None --",
                            Lane_Name = l.LaneName ?? "-- None --",
                            Method_of_Payment = pm.Description ?? "-- None --",
                            Toll_Collector_Class = tc1.ClassDescription,
                            AVC_Class = tc2.ClassDescription,
                            Final_Class = tc3.ClassDescription,
                           // Tariff = tpd.AmountInclusive,
                            Tariff = Convert.ToDecimal(tpd.AmountInclusive),
                            Tac_Card_Number = t.CardNumber
                        };

            // Execute query asynchronously
            return await query.ToListAsync();
        }


        // ==================== LOOKUP QUERIES ====================

        /// <summary>
        /// Returns distinct shift descriptions.
        /// Equivalent SQL: SELECT DISTINCT Description FROM Shift ORDER BY Description;
        /// </summary>
        public async Task<IEnumerable<string>> GetShiftsAsync()
        {
            return await _context.Shifts
                                 .Select(s => s.Description)
                                 .Distinct()
                                 .OrderBy(s => s) // ORDER BY
                                 .ToListAsync();
        }

        /// <summary>
        /// Returns distinct usernames.
        /// Equivalent SQL: SELECT DISTINCT Username FROM SystemUser;
        /// </summary>
        public async Task<IEnumerable<string>> GetTollOperatorsAsync()
        {
            return await _context.SystemUsers
                                 .Select(su => su.Username)
                                 .Distinct()
                                 .OrderBy(su => su) // Optional ordering
                                 .ToListAsync();
        }

        /// <summary>
        /// Returns distinct lane names.
        /// Equivalent SQL: SELECT DISTINCT LaneName FROM Lane;
        /// </summary>
        public async Task<IEnumerable<string>> GetLanesAsync()
        {
            return await _context.Lanes
                                 .Select(l => l.LaneName)
                                 .Distinct()
                                 .OrderBy(l => l)
                                 .ToListAsync();
        }

        /// <summary>
        /// Returns distinct payment method descriptions.
        /// Equivalent SQL: SELECT DISTINCT Description FROM PaymentMethod;
        /// </summary>
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
