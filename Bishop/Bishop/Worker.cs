using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bishop.Service;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bishop
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IBishopService _bishopService;

        public Worker(IBishopService bishopService, ILogger<Worker> logger)
        {
            _logger = logger;
            _bishopService = bishopService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _bishopService.PerformNextActionAsync();
            }
        }
    }
}
