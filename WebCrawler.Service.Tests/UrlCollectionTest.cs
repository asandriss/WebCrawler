using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Xunit;

namespace WebCrawler.Service.Tests;

[TestSubject(typeof(UrlCollection))]
public class UrlCollectionTest
{
    private readonly UrlCollection _collection = new UrlCollection(NullLogger<UrlCollection>.Instance);
    
    [Fact]
    public void Add_NewUrl_ShouldAddToPending()
    {
        var url = "http://testing.com";
        
        _collection.Add(url);
        
        _collection.HasNext().ShouldBeTrue();
        var next = _collection.GetNext();
        
        next.ShouldBe(url);
    }

    [Fact]
    public void Add_DuplicateUrl_ShouldNotBeAddedTwice()
    {
        var url = "https://test.com";
        
        _collection.Add(url);
        _collection.Add(url);

        // discard the first element
        _ = _collection.GetNext();
        
        _collection.HasNext().ShouldBeFalse();
    }

    [Fact]
    public void Add_MultipleUrls_ShouldAllBeAdded()
    {
        var urls = new[] { "https://testing.com", "https://testing.com/page", "https://other.com" };

        foreach (var url in urls)
        {
            _collection.Add(url);
        }

        var actual = urls.Select(x => x).ToArray();
        actual.ShouldBeEquivalentTo(urls);
    }

    [Fact]
    public void Add_VisitedUrl_ShouldNotAdd()
    {
        var url = "https://test.com";
        
        _collection.Add(url);
        var visted = _collection.GetNext();
        
        _collection.Add(url);
        
        visted.ShouldBe(url);
        _collection.HasNext().ShouldBeFalse();
    }

    [Fact]
    public void Add_SameUrlTwice_ShouldVisitOnceAndDisplayCorrectCount()
    {
        const string baseUrl = "https://testing.com";
        const string page1 = $"{baseUrl}/page1";

        var urls = new[] { baseUrl, page1, page1, page1.ToUpperInvariant() }; 
        foreach (var url in urls)
        {
            _collection.Add(url);
        }
        
        _collection.GetNext().ShouldBe(baseUrl);
        _collection.GetNext().ShouldBe(page1);
        _collection.HasNext().ShouldBeFalse();
        
        _collection.GetSeenCount(baseUrl).ShouldBe(1);
        _collection.GetSeenCount(page1).ShouldBe(3);
        _collection.GetSeenCount(page1.ToUpperInvariant()).ShouldBe(3);
        _collection.GetSeenCount("Non-existent").ShouldBe(0);
    }

    [Fact]
    public void GetAllSeenCounts_ShouldReturnVisitedLinksWithCorrectCounts()
    {
        const string baseUrl = "http://test.com";
        const string extBaseUrl = "http://external.com";
        const string page1 = $"{baseUrl}/page1";
        const string page1a = $"{baseUrl}/Page1/";
        const string page2 = $"{baseUrl}/PAGE2/";
        
        _collection.Add(baseUrl);
        _collection.Add(page1);
        _collection.Add(page1a);
        _collection.Add(page1a); // intentionally added it twice
        _collection.Add(page2);
        _collection.Add(extBaseUrl);

        while (_collection.HasNext())
        {
            _ = _collection.GetNext();
        }
        
        var actual = _collection.GetAllSeenCounts().ToArray();
        
        actual.FirstOrDefault(a => a.Url == baseUrl).Count.ShouldBe(1);
        actual.FirstOrDefault(a => a.Url == page1).Count.ShouldBe(3);
        actual.FirstOrDefault(a => a.Url == page2).Count.ShouldBe(1);
        actual.FirstOrDefault(a => a.Url == extBaseUrl).Count.ShouldBe(1);
    }
}