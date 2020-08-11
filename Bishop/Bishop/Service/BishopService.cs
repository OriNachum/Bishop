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
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly TimeSpan SleepTime = TimeSpan.FromSeconds(10);
        private DateTime? LastRun;
        private bool disposedValue;

        public BishopService(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task PerformNextActionAsync()
        {
            if (!LastRun.HasValue || (DateTime.UtcNow - LastRun) > SleepTime )
            {
                await NextCycleActionAsync();
                LastRun = DateTime.UtcNow;
            }
        }

        private async Task NextCycleActionAsync()
        {
            bool success = false;
            string request = "https://localhost:8001/ThermalAlert";
            _logger.LogDebug($"{nameof(BishopService)} - {nameof(NextCycleActionAsync)} - httpClient Request {request}");
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(request);
                _logger.LogDebug($"{nameof(BishopService)} - {nameof(NextCycleActionAsync)} - httpClient response {response}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"{nameof(BishopService)} - {nameof(NextCycleActionAsync)} - Error in request for next action. request: { request } response: {response}");
                    return;
                }
                string responseString = await response.Content.ReadAsStringAsync();
                success = JsonConvert.DeserializeObject<bool>(responseString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(BishopService)} - {nameof(NextCycleActionAsync)} - Error with httpClinet");
            }

            if (!success)
            {
                _logger.LogInformation($"{nameof(BishopService)} - {nameof(NextCycleActionAsync)} - no action to perform");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }

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
