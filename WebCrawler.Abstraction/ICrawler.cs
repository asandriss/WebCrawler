namespace WebCrawler.Abstraction;

public interface ICrawler
{
    Task RunAsync(string startingUrl);
}