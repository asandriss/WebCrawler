using System.Runtime.InteropServices.JavaScript;
using WebCrawler.Abstraction;

namespace WebCrawler.Service;

public class UrlCollection : IUrlCollection
{
    private readonly Queue<string> _pending = new();
    private readonly HashSet<string> _visited = [];
    private readonly Dictionary<string, int> _normalizedSeenCounts = new();

    public void Add(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return;

        var normalizedUri = uri.NormalizeUri();

        if (_normalizedSeenCounts.TryGetValue(normalizedUri, out int value))
        {
            _normalizedSeenCounts[normalizedUri] = ++value;
            return;
        }

        _normalizedSeenCounts[normalizedUri] = 1;
        _pending.Enqueue(url);
    }

    public bool HasNext() => _pending.Count > 0;

    public string GetNext()
    {
        var next = _pending.Dequeue();

        if (!next.IsValidUrl()) return next;
            
        _visited.Add(next);

        return next;
    }

    public IEnumerable<string> VisitedUrl => _visited;

    public int GetSeenCount(string url)
    {
        if(!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return 0;
        
        var normalizedUri = uri.NormalizeUri();
        
        return _normalizedSeenCounts.GetValueOrDefault(normalizedUri, 0);
    }
}