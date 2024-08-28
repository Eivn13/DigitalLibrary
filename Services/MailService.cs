using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class MailService : BackgroundService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MailService> _logger;
    
    public MailService(ILogger<MailService> logger)
    {
        _httpClient = new HttpClient();
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CallEndpointAsync();

                var nextRunTime = DateTime.Today.AddDays(1).AddHours(0);
                var delay = nextRunTime - DateTime.Now;
                if (delay < TimeSpan.Zero)
                {
                    delay = TimeSpan.FromDays(1);
                }

                await Task.Delay(delay, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing the daily task.");
            }
        }
    }

    private async Task CallEndpointAsync()
    {
        try
        {
            string url = "http://localhost:5212/Book/checknotices";

            
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully called the endpoint.");
            }
            else
            {
                _logger.LogWarning($"Failed to call the endpoint. Status code: {response.StatusCode}");
            }

            if (response is not null)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Sending notice to these emails: " + responseBody);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while calling the endpoint.");
        }
    }
}
