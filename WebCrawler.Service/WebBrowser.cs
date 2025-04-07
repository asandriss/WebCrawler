using WebCrawler.Abstraction;

namespace WebCrawler.Service;

public class WebBrowser(HttpClient client) : IWebBrowser
{
    public async Task<string?> GetPageHtml(string url)
    {
        try
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
        catch
        {
            // TODO: Logging and exception handling
            return null;
        }
    }
}