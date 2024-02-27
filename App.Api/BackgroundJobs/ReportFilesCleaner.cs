using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace App.Api.BackgroundJobs
{
    public class ReportFilesCleaner : BackgroundService
    {
        private readonly ILogger<ReportFilesCleaner> _logger;
        private readonly IConfiguration _configuration;
        public IServiceProvider Services { get; }

        public ReportFilesCleaner(IServiceProvider services,
            ILogger<ReportFilesCleaner> logger,
            IConfiguration configuration)
        {
            Services = services;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service running.");

            await DoWork(stoppingToken);
        }
        private async Task DoWork(CancellationToken stoppingToken)
        {
            await Task.Delay((int)TimeSpan.FromSeconds(10).TotalMilliseconds, stoppingToken);
            var filePath = Path.Combine(_configuration["ApplicationSetting:FilesRootPath"]);
            string[] filesArrays = Directory.GetFiles(filePath);



            DoWork(stoppingToken);

        }
    }
}
