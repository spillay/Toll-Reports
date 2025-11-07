using Microsoft.EntityFrameworkCore;
using Toll.Reporting.Api.DTOs;
using Toll.Reporting.Api.Repositories;
using TollReportingSystem.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

public class TopUpRepository : ITopUpRepository
{
    private readonly ApplicationDbContext _context;

    public TopUpRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<TopUpDto>> GetTopUpsAsync(
        DateTime startDate,
        DateTime endDate,
        string? operatorId = null,
        string? lane = null,
        string? shift = null,
        string? accountNumber = null,
        bool? operationalDate = null,
        int page = 1,
        int pageSize = 50)
    {
        var query =
            from rut in _context.RegisteredUserTopUps

            join ru in _context.RegisteredUsers
                on rut.RegisterUserId equals ru.RegisterUserId into ruGroup
            from ru in ruGroup.DefaultIfEmpty()

            join pm in _context.PaymentMethods
                on rut.PaymentMethodId equals pm.PaymentMethodId into pmGroup
            from pm in pmGroup.DefaultIfEmpty()

            where rut.RechargedOn >= startDate && rut.RechargedOn <= endDate
            select new
            {
                rut.RegisteredUserTopUpId,
                rut.RechargedOn,
                rut.RechargeStation,
                rut.RechargeShift,
                rut.SystemUserId,  
                ru.RegisterUserId,
                ru.AccNr,
                ru.CompanyName,
                rut.Amount,
                PaymentDesc = pm.Description
            };

        var totalCount = await query.CountAsync();

        var resultList = await query
            .OrderByDescending(x => x.RechargedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var items = resultList.Select(x => new TopUpDto
        {
            TopUpNumber = Convert.ToInt32(x.RegisteredUserTopUpId),
            TopUpDateTime = x.RechargedOn,
            LaneWorkstation = x.RechargeStation ?? string.Empty,
            Shift = x.RechargeShift.ToString() ?? string.Empty,

            Operator = x.SystemUserId.ToString(),

            AccountNumber = !string.IsNullOrEmpty(x.AccNr)
                ? x.AccNr
                : x.RegisterUserId.ToString(),

            AccountName = x.CompanyName ?? string.Empty,
            AmountPaid = Convert.ToDecimal(x.Amount),
            MethodOfPayment = x.PaymentDesc ?? string.Empty
        }).ToList();

        if (!string.IsNullOrWhiteSpace(operatorId))
            items = items
                .Where(x => x.Operator.Equals(operatorId, StringComparison.OrdinalIgnoreCase))
                .ToList();

        if (!string.IsNullOrWhiteSpace(lane))
            items = items
                .Where(x => x.LaneWorkstation.Equals(lane, StringComparison.OrdinalIgnoreCase))
                .ToList();

        if (!string.IsNullOrWhiteSpace(shift))
            items = items
                .Where(x => x.Shift.Equals(shift, StringComparison.OrdinalIgnoreCase))
                .ToList();

        if (!string.IsNullOrWhiteSpace(accountNumber))
            items = items
                .Where(x => x.AccountNumber.Equals(accountNumber, StringComparison.OrdinalIgnoreCase))
                .ToList();

        return new PagedResult<TopUpDto>
        {
            TotalCount = totalCount,
            Items = items
        };
    }
}
