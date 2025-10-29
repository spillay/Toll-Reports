using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Toll.Reporting.Api.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Toll.Reporting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _repo;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ITransactionRepository repo, ILogger<TransactionController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetTransactionDetails(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] List<string>? operationalShift,
            [FromQuery] List<string>? tollOperators,
            [FromQuery] List<string>? laneNames,
            [FromQuery] List<string>? paymentMethods,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Fetching transactions from {Start} to {End}, page {Page}, size {Size}", startDate, endDate, page, pageSize);
            _logger.LogInformation("Filters => Shifts: {Shifts}, Operators: {Operators}, Lanes: {Lanes}, PaymentMethods: {Payments}",
                string.Join(",", operationalShift ?? new List<string>()),
                string.Join(",", tollOperators ?? new List<string>()),
                string.Join(",", laneNames ?? new List<string>()),
                string.Join(",", paymentMethods ?? new List<string>()));

            var result = await _repo.GetTransactionDetailsAsync(
                startDate,
                endDate,
                operationalShift,
                tollOperators,
                laneNames,
                paymentMethods,
                page,
                pageSize
            );

            _logger.LogInformation("Total transactions found: {Count}", result.TotalCount);
            return Ok(result);
        }
        [HttpGet("filter-options")]
        public async Task<IActionResult> GetTransactionFilterOptions(
            DateTime startDate,
            DateTime endDate,
            [FromQuery] List<string>? operationalShift,
            [FromQuery] List<string>? tollOperators,
            [FromQuery] List<string>? laneNames,
            [FromQuery] List<string>? paymentMethods)
                {
                    var options = await _repo.GetTransactionFilterOptionsAsync(
                        startDate, endDate, operationalShift, tollOperators, laneNames, paymentMethods);

                    return Ok(options);
                }

    }


}