using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public class LoggerService : BackgroundService
    {
        private readonly ILogger<LoggerService> _logger;

        public LoggerService(ILogger<LoggerService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("LoggerService is starting.");

            stoppingToken.Register(() => _logger.LogInformation("LoggerService is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("LoggerService is doing background work.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            _logger.LogInformation("LoggerService has stopped.");
        }
    }
}
