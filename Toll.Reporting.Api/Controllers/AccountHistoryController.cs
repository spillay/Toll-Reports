using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TollReportingSystem.Data;

namespace Toll.Reporting.Api.Controllers
{
    [Authorize]
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

        [HttpGet("details")]
        public async Task<IActionResult> GetAccountHistory(
            [FromQuery] string? accountNumber = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountNumber) && !startDate.HasValue && !endDate.HasValue)
                    return BadRequest("At least one filter (account number OR start/end date) must be provided.");

                var start = startDate ?? DateTime.MinValue;
                var end = endDate ?? DateTime.MaxValue;

                var result = await _repository.GetAccountHistoryAsync(
                    accountNumber ?? string.Empty,
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

        [HttpGet("search-accounts")]
        public async Task<IActionResult> SearchAccounts([FromQuery] string q, [FromQuery] int take = 20)
        {
            q = (q ?? "").Trim();
            if (q.Length < 2)
                return Ok(Array.Empty<object>());

            take = Math.Clamp(take, 1, 50);

            // ---------------------------
            // DIGITS: prefix range search
            // ---------------------------
            if (long.TryParse(q, out var prefix))
            {
                // Your IDs look like 6 digits (116407, 169323, etc.)
                const int totalDigits = 6;

                // If user typed too many digits, fallback (rare case)
                if (q.Length >= totalDigits)
                {
                    // If you want strict equality instead, use u.RegisterUserId == prefix
                    var resultsExact = await _context.RegisteredUsers
                        .AsNoTracking()
                        .Where(u => u.RegisterUserId.ToString().Contains(q)) // fallback only
                        .OrderBy(u => u.RegisterUserId)
                        .Take(take)
                        .Select(u => new
                        {
                            accountNumber = u.RegisterUserId.ToString(),
                            description = (u.CompanyName ?? "") != ""
                                ? (u.RegisterUserId + " - " + u.CompanyName)
                                : (u.RegisterUserId + " - " + (u.FirstName ?? "") + " " + (u.LastName ?? "")),
                            balance = (decimal)u.Balance
                        })
                        .ToListAsync();

                    return Ok(resultsExact);
                }

                var factor = (long)Math.Pow(10, totalDigits - q.Length); // q=1164 => 100
                var min = prefix * factor;                               // 116400
                var max = (prefix + 1) * factor;                         // 116500

                var resultsDigits = await _context.RegisteredUsers
                    .AsNoTracking()
                    .Where(u => u.RegisterUserId >= min && u.RegisterUserId < max)
                    .OrderBy(u => u.RegisterUserId)
                    .Take(take)
                    .Select(u => new
                    {
                        accountNumber = u.RegisterUserId.ToString(),
                        description = (u.CompanyName ?? "") != ""
                            ? (u.RegisterUserId + " - " + u.CompanyName)
                            : (u.RegisterUserId + " - " + (u.FirstName ?? "") + " " + (u.LastName ?? "")),
                        balance = (decimal)u.Balance
                    })
                    .ToListAsync();

                return Ok(resultsDigits);
            }

            // ---------------------------
            // TEXT: name/company search
            // ---------------------------
            var like = $"%{q}%";

            var resultsText = await _context.RegisteredUsers
                .AsNoTracking()
                .Where(u =>
                    EF.Functions.Like(u.CompanyName ?? "", like) ||
                    EF.Functions.Like(u.FirstName ?? "", like) ||
                    EF.Functions.Like(u.LastName ?? "", like)
                )
                .OrderBy(u => u.RegisterUserId)
                .Take(take)
                .Select(u => new
                {
                    accountNumber = u.RegisterUserId.ToString(),
                    description = (u.CompanyName ?? "") != ""
                        ? (u.RegisterUserId + " - " + u.CompanyName)
                        : (u.RegisterUserId + " - " + (u.FirstName ?? "") + " " + (u.LastName ?? "")),
                    balance = (decimal)u.Balance
                })
                .ToListAsync();

            return Ok(resultsText);
        }
    }
}