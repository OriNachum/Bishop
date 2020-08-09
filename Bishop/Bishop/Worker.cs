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
        private readonly HttpClient _httpClient;
        private readonly IServiceProvider _services;
        private BishopService _bishopService;

        public Worker(ILogger<Worker> logger, IHttpClientProvider httpClientProvider, IServiceProvider services = null)
        {
            _logger = logger;
            _httpClient = httpClientProvider.GetHttpClient();
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _bishopService?.Dispose();
                _bishopService = new BishopService(_httpClient, this._logger);
                await _bishopService.StartAsync();
            }
        }
    }
}
