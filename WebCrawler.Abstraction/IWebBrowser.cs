namespace WebCrawler.Abstraction;

public interface IWebBrowser
{
   Task<string?> GetPageHtml(string url, int timeOutInSeconds = 30);
}