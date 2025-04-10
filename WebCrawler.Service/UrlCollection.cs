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
        
        // this is a very simplistic way of tracking the visited URLs.
        //  one of the first improvements would make this more robust
        _visited.Add(next);
        
        return next;
    }

    public IEnumerable<string> VisitedUrl => _visited;
}