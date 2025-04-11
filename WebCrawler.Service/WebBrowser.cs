using System.Net;
using Microsoft.Extensions.Logging;
using WebCrawler.Abstraction;

namespace WebCrawler.Service;

public class WebBrowser(HttpClient client, ILogger<WebBrowser> logger) : IWebBrowser
{
    public async Task<string?> GetPageHtml(string url, int timeOutInSeconds = 30)
    {
        logger.LogInformation("Sending HTTP request to URL: [{Url}]", url);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeOutInSeconds));
        try
        {
            var response = await client.GetAsync(url, cts.Token);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogWarning("Page not found at URL: [{Url}]", url);
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            logger.LogError("HTTP request to [{Url}] timed out.", url);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "HTTP request to [{Url}] failed.", url);
            return null;
        }
    }
}