using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toll.Reporting.Api.DTOs;
using TollReportingSystem.Data;

public class ComprehensiveRepository : IComprehensiveRepository
{
    private readonly ApplicationDbContext _context;

    public ComprehensiveRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ComprehensiveOptionsDto> GetComprehensiveOptionsAsync()
    {
      
        var shifts = await _context.Shifts.AsNoTracking()
            .Select(x => new FilterOptionDto<byte>
            {
                Id = (byte)x.ShiftId,
                Name = x.Description ?? x.ShiftId.ToString()
            })
            .OrderBy(x => x.Name)
            .ToListAsync();

        var operators = await _context.SystemUsers.AsNoTracking()
            .Select(x => new FilterOptionDto<long>
            {
                Id = x.SystemUserId,
                Name = x.Username ?? x.SystemUserId.ToString()
            })
            .OrderBy(x => x.Name)
            .ToListAsync();

        var lanes = await _context.Lanes.AsNoTracking()
            .Select(x => new FilterOptionDto<int>
            {
                Id = x.LaneId,
                Name = x.LaneName ?? x.LaneId.ToString()
            })
            .OrderBy(x => x.Name)
            .ToListAsync();

        var discountTypes = await _context.DiscountTypes.AsNoTracking()
            .Select(x => new FilterOptionDto<byte>
            {
                Id = (byte)x.DiscountTypeId,
                Name = x.Description ?? x.DiscountTypeId.ToString()
            })
            .OrderBy(x => x.Name)
            .ToListAsync();

        var tollClasses = await _context.TollClasses.AsNoTracking()
            .Select(x => new FilterOptionDto<byte>
            {
                Id = (byte)x.TollClassId,
                Name = x.ClassDescription ?? x.TollClassId.ToString()
            })
            .OrderBy(x => x.Name)
            .ToListAsync();

        //  Payment Method = TransactionTypes
        var paymentMethods = await _context.TransactionTypes.AsNoTracking()
            .Select(x => new FilterOptionDto<byte>
            {
                Id = (byte)x.TransactionTypeId,
                Name = x.Description ?? x.TransactionTypeId.ToString()
            })
            .OrderBy(x => x.Name)
            .ToListAsync();

        return new ComprehensiveOptionsDto
        {
            Shifts = shifts,
            Operators = operators,
            Lanes = lanes,
            DiscountTypes = discountTypes,
            TollClasses = tollClasses,
            PaymentMethods = paymentMethods
        };
    }

    public async Task<IEnumerable<ComprehensiveDto>> GetComprehensiveRepositoryAsync(
    DateTime startDate,
    DateTime endDate,
    List<byte>? shiftIds = null,
    List<long>? operatorIds = null,
    List<int>? laneIds = null,
    List<byte>? discountTypeIds = null,
    List<byte>? tollClassIds = null,
    List<byte>? paymentMethodIds = null)
    {
        // Treat empty list as "no filter"
        static List<T>? NullIfEmpty<T>(List<T>? list) =>
            (list != null && list.Count == 0) ? null : list;

        shiftIds = NullIfEmpty(shiftIds);
        operatorIds = NullIfEmpty(operatorIds);
        laneIds = NullIfEmpty(laneIds);
        discountTypeIds = NullIfEmpty(discountTypeIds);
        tollClassIds = NullIfEmpty(tollClassIds);
        paymentMethodIds = NullIfEmpty(paymentMethodIds);
        var start = startDate.Date;
        var endExclusive = endDate.Date.AddDays(1);
        //  MAIN QUERY (IQueryable, filters apply in SQL)
        var query =
            from t in _context.Transactions.AsNoTracking()

            join s in _context.Shifts.AsNoTracking()
                on (int?)t.ShiftId equals (int?)s.ShiftId into sGroup
            from s in sGroup.DefaultIfEmpty()

            join u in _context.SystemUsers.AsNoTracking()
                on (long?)t.SystemUserId equals (long?)u.SystemUserId into uGroup
            from u in uGroup.DefaultIfEmpty()

            join l in _context.Lanes.AsNoTracking()
                on (int?)t.LaneId equals (int?)l.LaneId into lGroup
            from l in lGroup.DefaultIfEmpty()

                //  Payment Method lookup: TransactionTypeId -> TransactionTypes.Description
            join pm in _context.TransactionTypes.AsNoTracking()
                on (int?)t.TransactionTypeId equals (int?)pm.TransactionTypeId into pmGroup
            from pm in pmGroup.DefaultIfEmpty()

            join d in _context.DiscountTypes.AsNoTracking()
                on (int?)t.DiscountTypeId equals (int?)d.DiscountTypeId into dGroup
            from d in dGroup.DefaultIfEmpty()

            join tc in _context.TollClasses.AsNoTracking()
                on (int?)t.ManualTollClassId equals (int?)tc.TollClassId into tcGroup
            from tc in tcGroup.DefaultIfEmpty()

            join tpd in _context.TariffPlanDetails.AsNoTracking()
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

           

        where t.TransactionDateTime >= start
           && t.TransactionDateTime < endExclusive

            select new
            {
                // IDs
                t.LaneId,
                t.DiscountTypeId,
                t.ShiftId,
                t.ManualTollClassId,
                t.SystemUserId,
                PaymentMethodId = t.TransactionTypeId, 

                // Names
                ShiftName = s.Description,
                OperatorName = u.Username,
                LaneName = l.LaneName,
                DiscountTypeName = d.Description,
                ManualTollClassName = tc.ClassDescription,
                PaymentMethodName = pm.Description,

                // Other
                t.TransactionDateTime,
                t.TariffPlanId,
                AmountInclusive = tpd.AmountInclusive
            };

        //  Apply multi-select filters in SQL

        if (shiftIds?.Any() == true)
            query = query.Where(x => shiftIds.Contains(x.ShiftId));

        if (operatorIds?.Any() == true)
            query = query.Where(x => operatorIds.Contains(x.SystemUserId.Value));

        if (laneIds?.Any() == true)
            query = query.Where(x => laneIds.Contains(x.LaneId));

        if (discountTypeIds?.Any() == true)
            query = query.Where(x => discountTypeIds.Contains(x.DiscountTypeId));

        if (tollClassIds?.Any() == true)
            query = query.Where(x => tollClassIds.Contains(x.ManualTollClassId));

        if (paymentMethodIds?.Any() == true)
            query = query.Where(x => paymentMethodIds.Contains(x.PaymentMethodId));

        // Map to DTO
        var result = await query
            .OrderByDescending(x => x.TransactionDateTime)
            .Select(x => new ComprehensiveDto
            {
                LaneId = x.LaneId,
                DiscountTypeId = x.DiscountTypeId,
                ShiftId = x.ShiftId,
                ManualTollClassId = x.ManualTollClassId,
                TollOperatorId = x.SystemUserId,

                LaneName = x.LaneName,
                DiscountType = x.DiscountTypeName,
                TransactionDateTime = x.TransactionDateTime,
                ShiftName = x.ShiftName,
                ManualTollClassName = x.ManualTollClassName,
                PaymentMethodId = x.PaymentMethodId,
                PaymentMethodName = x.PaymentMethodName,
                TollOperatorName = x.OperatorName,

                TariffPlanId = x.TariffPlanId,
                AmountInclusive = x.AmountInclusive
            })
            .ToListAsync();

        return result;
    }
}