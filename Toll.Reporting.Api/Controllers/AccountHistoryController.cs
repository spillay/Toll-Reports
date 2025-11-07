using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TollReportingSystem.Data;

[ApiController]
[Route("api/[controller]")]
public class AccountHistoryController : ControllerBase
{
    private readonly IAccountHistoryRepository _repository;
    private readonly ApplicationDbContext _context;

    public AccountHistoryController(IAccountHistoryRepository repository, ApplicationDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    /// <summary>
    /// ✅ Get full account history report (single account or all)
    /// </summary>
    [HttpGet("details")]
    public async Task<IActionResult> GetAccountHistory([FromQuery] string? accountNumber = null)
    {
        try
        {
            // 🔹 If no account is provided, return all accounts' history
            var result = await _repository.GetAccountHistoryAsync(accountNumber);

            // 🟡 Handle "no data" case gracefully
            if ((result.AccountHeader == null) && (result.HistoryRecords == null || !result.HistoryRecords.Any()))
            {
                return NotFound(new { message = "No account history found." });
            }

            // ✅ Return standard JSON response
            return Ok(new
            {
                accountHeader = result.AccountHeader,
                historyRecords = result.HistoryRecords
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "An error occurred while fetching the account history report.",
                details = ex.Message
            });
        }
    }

    /// <summary>
    /// ✅ Fetch all registered account numbers for dropdown
    /// </summary>
    [HttpGet("accounts")]
    public async Task<IActionResult> GetAccounts()
    {
        try
        {
            // Step 1️⃣ – Pull only raw account numbers (no DISTINCT / LTRIM / RTRIM in SQL)
            var rawAccounts = await _context.RegisteredUsers
                .AsNoTracking()
                .Select(u => u.AccNr)
                .Where(acc => acc != null && acc != "")
                .ToListAsync();

            // Step 2️⃣ – Process in memory
            var accounts = rawAccounts
                .AsParallel()
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .Select(a => a.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(a => a)
                .Take(5000)
                .ToList();

            return Ok(accounts);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "An error occurred while fetching the account list.",
                details = ex.Message
            });
        }
    }
}
