using System.Linq;
using JetBrains.Annotations;
using Shouldly;
using WebCrawler.Service;
using Xunit;

namespace WebCrawler.Service.Tests;

[TestSubject(typeof(UrlCollection))]
public class UrlCollectionTest
{
    private readonly UrlCollection _collection = new UrlCollection();
    
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
}