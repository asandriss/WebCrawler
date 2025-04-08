using System.Linq;
using JetBrains.Annotations;
using Shouldly;
using WebCrawler.Service;
using Xunit;

namespace WebCrawler.Service.Tests;

[TestSubject(typeof(UrlParser))]
public class UrlParserTest
{
    private readonly UrlParser _parser;
    private const string BaseUrl = "http://test.com";
        
    private const string HtmlContent = @"
            <html>
                <body>
                    <a href='http://test.com/page1'>Page 1</a>
                    <a href='https://test.com/page2/'>Page 2</a>
                    <a href='http://www.test.com/page3'>Page 3</a>
                    <a href='/page4'>Page 4</a>
                    <a href='PAGE5/'>Page 5</a>
                    <a href='https://external.com/page6'>External Page 6</a>
                    <a>No Href Link</a>
                    <a href=' '>Empty Href Link</a>
                </body>
            </html>";

    public UrlParserTest()
    {
        _parser = new UrlParser();
    }
    
    [Fact]
    public void GetUrlsFromHtmlContent_ShouldExtractAndNormalizeUrls()
    {
        var expectedUrls = new[]
        {
            "test.com/page1",
            "test.com/page2",
            "test.com/page3",
            "test.com/page4",
            "test.com/page5",
            "external.com/page6"
        };

        var actualUrls = _parser.GetUrlsFromHtmlContent(HtmlContent, BaseUrl).ToArray();
        actualUrls.ShouldBeEquivalentTo(expectedUrls);
    }
}