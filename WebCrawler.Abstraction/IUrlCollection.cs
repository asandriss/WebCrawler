namespace WebCrawler.Abstraction;

public interface IUrlCollection
{
    void Add(string url);
    bool HasNext();
    string GetNext();
}