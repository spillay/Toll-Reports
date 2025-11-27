using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using Toll.Reporting.Api.DTOs;
using Toll.Reporting.Api.Repositories.Interfaces;

namespace Toll.Reporting.Api.Repositories
{
    public class EndOfDayReportRepository : IEndOfDayReportRepository
    {
        private readonly string _connectionString;

        public EndOfDayReportRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SQLServerConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<List<EndOfDayRowDto>> GetEndOfDayReportAsync(DateTime reportDate)
        {
            using var conn = new SqlConnection(_connectionString);

            var result = await conn.QueryAsync<EndOfDayRowDto>(
                "star.usp_GenerateEndOfDayReport",
                new { ReportDate = reportDate },
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }
    }
}
