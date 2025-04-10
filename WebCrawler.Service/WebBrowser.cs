using Microsoft.Extensions.Logging;
using WebCrawler.Abstraction;

namespace WebCrawler.Service;

public class WebBrowser(HttpClient client, ILogger<WebBrowser> logger) : IWebBrowser
{
    public async Task<string?> GetPageHtml(string url)
    {
        logger.LogInformation("Sending HTTP request to URL: [{Url}]", url);
        
        try
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
        catch(Exception ex)
        {
            logger.LogError("HTTP request to [{Url}] failed. Exception occured: {ex}", url, ex);
            return null;
        }
    }
}