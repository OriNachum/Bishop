using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bishop.Service
{
    public class BishopService : IBishopService
    {
        private readonly TimeSpan SleepTime = TimeSpan.FromSeconds(5);
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private ITimer timer;
        private bool disposedValue;

        public BishopService(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task StartAsync()
        {
            await NextCycleActionAsync();
            Func<Task> GenerateNextCycleAction = NextCycleActionAsync;
            ITimer oldTimer = timer;
            timer = new GapBasedTimer(GenerateNextCycleAction, SleepTime, _logger);
            oldTimer?.Dispose();
            timer.InitializeCallback(GenerateNextCycleAction, SleepTime);
        }

        private async Task NextCycleActionAsync()
        {
            bool success = false;
            string request = "https://localhost:8001/ThermalAlert";
            _logger.LogDebug($"{nameof(BishopService)} - NextCycleAction - httpClient Request {request}");
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(request);
                _logger.LogDebug($"{nameof(BishopService)} - NextCycleAction - httpClient response {response}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"{nameof(BishopService)} - NextCycleAction - Error in request for next action. request: { request } response: {response}");
                    return;
                }
                string responseString = await response.Content.ReadAsStringAsync();
                success = JsonConvert.DeserializeObject<bool>(responseString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(BishopService)} - NextCycleAction - Error with httpClinet");
            }

            if (!success)
            {
                _logger.LogInformation($"{nameof(BishopService)} - NextCycleAction - no action to perform");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                    timer?.Dispose();
                }

                timer = null;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}
