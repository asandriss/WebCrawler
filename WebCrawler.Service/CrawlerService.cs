using WebCrawler.Abstraction;

namespace WebCrawler.Service;

public class CrawlerService : ICrawler
{
    private readonly IUrlParser _parser;
    private readonly IUrlCollection _collection;
    private readonly IWebBrowser _browser;

    public CrawlerService(IUrlParser parser, IUrlCollection collection, IWebBrowser browser)
    {
        _parser = parser;
        _collection = collection;
        _browser = browser;
    }
    
    public string EchoMe(string input)
    {
        return input;
    }

    public async Task RunAsync(string startingUrl)
    {
        if (!Uri.TryCreate(startingUrl, UriKind.Absolute, out var baseUrl))
        {
            Console.WriteLine($"Invalid starting URL: [{startingUrl}]");
            return;
        }

        var domain = baseUrl.Host;
        
        _collection.Add(startingUrl);

        while (_collection.HasNext())
        {
            var currentUrl = _collection.GetNext();
            if (!Uri.TryCreate(currentUrl, UriKind.Absolute, out var currentUri))
                continue;
            
            if(currentUri.Host != domain)
                continue;
            
            var currentContent = await _browser.GetPageHtml(currentUrl);

            if (currentContent is null)
            {
                Console.WriteLine($"Failed to fetch content for: [{currentUrl}]");
                continue;
            }

            var linksOnPage = _parser
                .GetUrlsFromHtmlContent(currentContent, baseUrl.ToString())
                .ToArray();
            
            foreach (var link in linksOnPage)
            {
                _collection.Add(link);
            }
        }
    }
}