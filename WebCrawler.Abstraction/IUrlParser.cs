namespace WebCrawler.Abstraction;

public interface IUrlParser
{
    IEnumerable<string> GetUrlsFromHtmlContent(string htmlContent, string baseUrl);
}