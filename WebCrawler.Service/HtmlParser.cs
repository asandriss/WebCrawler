using AngleSharp.Html.Parser;
using WebCrawler.Abstraction;

namespace WebCrawler.Service;

public class UrlParser : IUrlParser
{
    private readonly HtmlParser _parser = new HtmlParser();
    
    public IEnumerable<string> GetUrlsFromHtmlContent(string htmlContent, string baseUrl)
    {
        var document = _parser.ParseDocument(htmlContent);
        var links = document.QuerySelectorAll("a[href]")
            .Select(el => el.GetAttribute("href"))
            .Where(href => !string.IsNullOrWhiteSpace(href));

        return links
            .Select(l => l.NormalizeUrl(baseUrl))
            .Where(url => !string.IsNullOrWhiteSpace(url));
    }
}