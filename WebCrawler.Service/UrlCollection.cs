using WebCrawler.Abstraction;

namespace WebCrawler.Service;

public class UrlCollection : IUrlCollection
{
    private readonly Queue<string> _pending = new();
    
    public void Add(string url)
    {
        if(!_pending.Contains(url))
            _pending.Enqueue(url);
    }

    public bool HasNext() => _pending.Count > 0;

    public string GetNext()
    {
        var next = _pending.Dequeue();
        return next;
    }
}