using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TollReportingSystem.Data;
using Toll.Reporting.Api.DTOs;
using Toll.Reporting.Api.Repositories.Interfaces;

namespace Toll.Reporting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountHistoryController : ControllerBase
    {
        private readonly IAccountHistoryRepository _repository;
        private readonly ApplicationDbContext _context;

        public AccountHistoryController(
            IAccountHistoryRepository repository,
            ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        // ================================================================================
        // ✅ ACCOUNT HISTORY REPORT (MAIN ENDPOINT)
        // ================================================================================
        [HttpGet("details")]
        public async Task<IActionResult> GetAccountHistory(
            [FromQuery] string? accountNumber = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                // Basic validation – ensure at least one filter is present
                if (string.IsNullOrWhiteSpace(accountNumber)
                    && !startDate.HasValue
                    && !endDate.HasValue)
                {
                    return BadRequest("At least one filter (account number OR start/end date) must be provided.");
                }

                // Normalize dates
                DateTime start = startDate ?? DateTime.MinValue;
                DateTime end = endDate ?? DateTime.MaxValue;

                var result = await _repository.GetAccountHistoryAsync(
                    accountNumber,
                    start,
                    end
                );

                return Ok(new
                {
                    accountHeader = result.AccountHeader,
                    historyRecords = result.HistoryRecords,
                    totalTopUps = result.TotalTopUps,
                    totalTransactions = result.TotalTransactions,
                    netMovement = result.NetMovement
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while fetching the account history.",
                    details = ex.Message
                });
            }
        }

        // ================================================================================
        // ✅ FETCH ACCOUNT LIST (USED FOR DROPDOWN)
        // ================================================================================
        [HttpGet("accounts")]
        public async Task<IActionResult> GetAccounts()
        {
            try
            {
                var rawAccounts = await _context.RegisteredUsers
                    .AsNoTracking()
                    .Select(u => u.RegisterUserId.ToString())
                    .ToListAsync();

                var accounts = rawAccounts
                    .Where(a => !string.IsNullOrWhiteSpace(a))
                    .Select(a => a.Trim())
                    .Distinct()
                    .OrderBy(a => a)
                    .ToList();

                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while loading account numbers.",
                    details = ex.Message
                });
            }
        }
    }
}
