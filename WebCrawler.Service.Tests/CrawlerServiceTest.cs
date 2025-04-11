using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Moq;
using Shouldly;
using WebCrawler.Abstraction;
using WebCrawler.Service;
using Xunit;
using Microsoft.Extensions.Logging.Abstractions;

namespace WebCrawler.Service.Tests;

[TestSubject(typeof(CrawlerService))]
public class CrawlerServiceTest
{
    private const string StartingUrl = "http://test.com";
    private readonly Mock<IWebBrowser> _browserMock = new();
    private readonly Mock<IUrlParser> _urlParserMock = new();
    private readonly Mock<IUrlCollection> _urlCollectionMock = new();
    private readonly CrawlerService _sut;

    public CrawlerServiceTest()
    {
        _sut = new CrawlerService(
            _urlParserMock.Object,
            _urlCollectionMock.Object,
            _browserMock.Object,
            NullLogger<CrawlerService>.Instance);
    }

    [Fact]
    public async Task RunAsync_ShouldCrawlThroughInternalLinksOnly()
    {
        var linksFromPage = new[]
        {
            "http://test.com/page1",
            "http://external.com/extPage1"
        };

        var queue = new Queue<string>([StartingUrl]);
        SetupCollectionMock(queue);
        SetupBrowserMock();
        SetupParserMock(linksFromPage);

        await _sut.RunAsync(StartingUrl);

        _urlCollectionMock.Verify(c => c.Add(StartingUrl), Times.Once);
        _urlCollectionMock.Verify(c => c.Add("http://test.com/page1"), Times.AtLeastOnce);
        _urlCollectionMock.Verify(c => c.Add("http://external.com/extPage1"), Times.AtLeastOnce);
        
        _browserMock.Verify(b => b.GetPageHtml("http://test.com/page1", It.IsAny<int>()), Times.Once);
        _browserMock.Verify(b => b.GetPageHtml("http://external.com/extPage1", It.IsAny<int>()), Times.Never);
    }

    private void SetupParserMock(string[] linksToReturn) =>
        _urlParserMock
            .Setup(p => p.GetUrlsFromHtmlContent(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(linksToReturn);


    private void SetupBrowserMock() =>
        _browserMock
            .Setup(b => b.GetPageHtml(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync("<html></html>");

    private void SetupCollectionMock(Queue<string> queue)
    {
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        _urlCollectionMock
            .Setup(c => c.Add(It.IsAny<string>()))
            .Callback<string>(url =>
            {
                if (!visited.Contains(url) && !queue.Contains(url))
                    queue.Enqueue(url);
            });

        _urlCollectionMock
            .Setup(c => c.HasNext())
            .Returns(() => queue.Count > 0);

        _urlCollectionMock
            .Setup(c => c.GetNext())
            .Returns(() =>
            {
                var next = queue.Dequeue();
                visited.Add(next); // mark as visited
                return next;
            });
    }

    [Fact]
    public void EchoMe_NewString_ShouldEcho()
    {
        var expect = "Echo me!";

        var actual = _sut.EchoMe(expect);
        actual.ShouldBe(expect);
    }
}