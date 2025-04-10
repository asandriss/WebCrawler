using Microsoft.Extensions.Logging;
using WebCrawler.Abstraction;

namespace WebCrawler.Service;

public class CrawlerService : ICrawler
{
    private readonly IUrlParser _parser;
    private readonly IUrlCollection _collection;
    private readonly IWebBrowser _browser;
    private readonly ILogger<CrawlerService> _logger;

    public CrawlerService(IUrlParser parser, IUrlCollection collection, IWebBrowser browser, ILogger<CrawlerService> logger)
    {
        _parser = parser;
        _collection = collection;
        _browser = browser;
        _logger = logger;
    }
    
    public string EchoMe(string input)
    {
        return input;
    }

    public async Task RunAsync(string startingUrl)
    {
        _logger.LogInformation("Crawler started with [{StartingUrl}]", startingUrl);
        
        if (!Uri.TryCreate(startingUrl, UriKind.Absolute, out var baseUrl))
        {
            _logger.LogWarning("Invalid starting URL: [{Url}]", startingUrl);
            return;
        }

        var domain = baseUrl.Host;
        
        _collection.Add(startingUrl);

        while (_collection.HasNext())
        {
            var currentUrl = _collection.GetNext();
            if (!Uri.TryCreate(currentUrl, UriKind.Absolute, out var currentUri))
                continue;

            if (currentUri.Host != domain)
            {
                _logger.LogInformation("External URL found [{Url}], will not crawl", currentUri.Host);
                continue;
            }
            
            var currentContent = await _browser.GetPageHtml(currentUrl);

            if (currentContent is null)
            {
                _logger.LogWarning("Failed to fetch content for: [{Url}]", currentUri.ToString());
                continue;
            }

            var linksOnPage = _parser
                .GetUrlsFromHtmlContent(currentContent, baseUrl.ToString())
                .ToArray();
            
            _logger.LogInformation("Found [{NumLinksOnPage}] links on page [{Url}]. Adding to collection.", 
                linksOnPage.Length,
                currentUrl );
            
            foreach (var link in linksOnPage.Where(l => !string.IsNullOrWhiteSpace(l)))
            {
                _collection.Add(link!);
            }
        }
    }
}