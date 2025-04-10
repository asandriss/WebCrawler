using AngleSharp.Html.Parser;
using Microsoft.Extensions.Logging;
using WebCrawler.Abstraction;

namespace WebCrawler.Service;

public class UrlParser(ILogger<UrlParser> logger) : IUrlParser
{
    private readonly HtmlParser _parser = new HtmlParser();

    public IEnumerable<string?> GetUrlsFromHtmlContent(string htmlContent, string baseUrl)
    {
        var document = _parser.ParseDocument(htmlContent);
        var anchorElements = document.QuerySelectorAll("a[href]");

        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri))
        {
            logger.LogWarning("Invalid base Url provided [{BaseUrl}]. Cannot parse html content.", baseUrl);
            yield break;
        }

        foreach (var element in anchorElements)
        {
            var href = element.GetAttribute("href");
            
            if(string.IsNullOrWhiteSpace(href))
                continue;

            if (!Uri.TryCreate(baseUri, href, out var createdUri))
            {
                logger.LogWarning("Could not construct a valid Uri from href [{Href}]", href);
                continue;
            }

            if (createdUri.Scheme != Uri.UriSchemeHttp && createdUri.Scheme != Uri.UriSchemeHttps)
            {
                logger.LogWarning("Constructed Uri [{Uri}] is not HTTP or HTTPS", createdUri.Scheme);
                continue;
            }

            yield return createdUri.ToString();
        }
    }
}