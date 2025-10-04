using Microsoft.EntityFrameworkCore;
using Toll.Reporting.Api.DTOs;
using TollReportingSystem.Data;

public class DiscrepancyRepository : IDiscrepancyRepository
{
    private readonly ApplicationDbContext _context;

    public DiscrepancyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DiscrepancyDto>> GetDiscrepancyAsync(
        DateTime startDate,
        DateTime endDate,
        List<string>? operationalShift = null,
        List<string>? tollOperators = null,
        List<string>? laneNames = null,
        List<string>? paymentMethods = null,
        List<string>? takenAction = null)
    {

        var query =
  from t in _context.Transactions

      // LEFT JOIN Shift
  join s in _context.Shifts
      on t.ShiftId equals s.ShiftId into shiftGroup
  from s in shiftGroup.DefaultIfEmpty()

      // LEFT JOIN SystemUser
  join su in _context.SystemUsers
      on t.SystemUserId equals su.SystemUserId into userGroup
  from su in userGroup.DefaultIfEmpty()

      // LEFT JOIN Lane
  join l in _context.Lanes
      on t.LaneId equals l.LaneId into laneGroup
  from l in laneGroup.DefaultIfEmpty()

      // LEFT JOIN TransactionType
  join tt in _context.TransactionTypes
      on t.TransactionTypeId equals tt.TransactionTypeId into typeGroup
  from tt in typeGroup.DefaultIfEmpty()

      // LEFT JOIN Manual Toll Class
  join tc1 in _context.TollClasses
      on t.ManualTollClassId equals tc1.TollClassId into tc1Group
  from tc1 in tc1Group.DefaultIfEmpty()

      // LEFT JOIN Automatic Toll Class
  join tc2 in _context.TollClasses
      on t.AutomaticTollClassId equals tc2.TollClassId into tc2Group
  from tc2 in tc2Group.DefaultIfEmpty()

      // LEFT JOIN Final Toll Class
  join tc3 in _context.TollClasses
      on t.ActualTollClassId equals tc3.TollClassId into tc3Group
  from tc3 in tc3Group.DefaultIfEmpty()

      // LEFT JOIN TariffPlanDetail
  join tpd in _context.TariffPlanDetails
      on new { t.TariffPlanId, TollClassId = t.ManualTollClassId }
      equals new { tpd.TariffPlanId, tpd.TollClassId } into tpdGroup
  from tpd in tpdGroup.DefaultIfEmpty()

      // Filter by date
  where t.TransactionDateTime >= startDate &&
        t.TransactionDateTime <= endDate

  select new DiscrepancyDto
  {
      Lane_Nr = t.LaneId,
      Trx_Sequence_Nr = t.TransactionNumber.ToString(),
      Trx_Date = t.TransactionDateTime.ToString("dd/MM/yyyy"),
      Trx_Time = t.TransactionDateTime.ToString("HH:mm:ss"),
      Operational_Shift = s != null ? s.Description : "-- None --",
      Toll_Operator_ID = su != null ? su.Username : "-- None --",
      Lane_Name = l != null ? l.LaneName : "-- None --",
      Method_of_Payment = tt != null ? tt.Description : "-- None --",
      Toll_Collector_Class = tc1 != null ? tc1.ClassDescription : null,
      AVC_Class = tc2 != null ? tc2.ClassDescription : null,
      Final_Class = tc3 != null ? tc3.ClassDescription : null,
      Tariff = tpd != null ? (decimal?)tpd.AmountInclusive : null,
      Updated_Tariff = tpd != null ? (decimal?)tpd.AmountExclusive : null
  };

        // Apply optional filters
        if (operationalShift != null && operationalShift.Any())
        {
            query = query.Where(x => operationalShift.Contains(x.Operational_Shift));
        }

        if (tollOperators != null && tollOperators.Any())
        {
            query = query.Where(x => tollOperators.Contains(x.Toll_Operator_ID));
        }

        if (laneNames != null && laneNames.Any())
        {
            query = query.Where(x => laneNames.Contains(x.Lane_Name));
        }

        if (paymentMethods != null && paymentMethods.Any())
        {
            query = query.Where(x => paymentMethods.Contains(x.Method_of_Payment));
        }

        return await query.ToListAsync();
    }

}
