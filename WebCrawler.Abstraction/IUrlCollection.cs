namespace WebCrawler.Abstraction;

public interface IUrlCollection
{
    void Add(string url);
    bool HasNext();
    string GetNext();
    
    IEnumerable<string> VisitedUrl { get; }

    int GetSeenCount(string url);
}
