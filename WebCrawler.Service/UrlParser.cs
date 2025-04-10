using AngleSharp.Html.Parser;
using WebCrawler.Abstraction;

namespace WebCrawler.Service;

public class UrlParser : IUrlParser
{
    private readonly HtmlParser _parser = new HtmlParser();

    public IEnumerable<string?> GetUrlsFromHtmlContent(string htmlContent, string baseUrl)
    {
        var document = _parser.ParseDocument(htmlContent);
        var anchorElements = document.QuerySelectorAll("a[href]");
        
        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri))
            yield break;
        
        foreach (var element in anchorElements )
        {
            var href = element.GetAttribute("href");
            
            if(string.IsNullOrWhiteSpace(href))
                continue;
            
            if(!Uri.TryCreate(baseUri, href, out var createdUri))
               continue;
               
            if(createdUri.Scheme != Uri.UriSchemeHttp && createdUri.Scheme != Uri.UriSchemeHttps)
                continue;

            yield return createdUri.ToString();
        }
    }
}