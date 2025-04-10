using WebCrawler.Abstraction;

namespace WebCrawler.Service;

public class UrlCollection : IUrlCollection
{
    private readonly Queue<string> _pending = new();
    private readonly HashSet<string> _visited = [];
    
    public void Add(string url)
    {
        if(!_pending.Contains(url) && !_visited.Contains(url))
            _pending.Enqueue(url);
    }

    public bool HasNext() => _pending.Count > 0;

    public string GetNext()
    {
        var next = _pending.Dequeue();
        
        _visited.Add(next);
        
        return next;
    }

    public IEnumerable<string> VisitedUrl => _visited;
}