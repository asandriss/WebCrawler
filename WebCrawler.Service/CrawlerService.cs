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
        var baseUrl = new Uri(startingUrl);
        var domain = baseUrl.Host;
        
        _collection.Add(startingUrl);

        while (_collection.HasNext())
        {
            var currentUrl = _collection.GetNext();
            var currentContent = await _browser.GetPageHtml(currentUrl);

            if (currentContent == null) continue;
            var linksOnPage = _parser.GetUrlsFromHtmlContent(currentContent, baseUrl.ToString()).ToArray();

            PrintToConsole(linksOnPage);

            foreach (var link in linksOnPage)
            {
                if (new Uri(link).Host == domain)
                {
                    _collection.Add(link);
                }
            }
        }
    }

    private void PrintToConsole(IEnumerable<string> linksOnPage)
    {
        foreach (var link in linksOnPage)
        {
            Console.WriteLine($"Found link {link}");
        }
    }
}