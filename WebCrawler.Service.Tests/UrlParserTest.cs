using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Xunit;

namespace WebCrawler.Service.Tests;

[TestSubject(typeof(UrlParser))]
public class UrlParserTest
{
    private readonly UrlParser _parser = new(NullLogger<UrlParser>.Instance);
    private const string BaseUrl = "http://test.com";
        
    private const string HtmlContent = @"
            <html>
                <body>
                    <a href='http://test.com/page1'>Page 1</a>
                    <a href='https://test.com/page2/'>Page 2</a>
                    <a href='http://www.test.com/page3/'>Page 3</a>
                    <a href='/page4//'>Page 4</a>
                    <a href='PAGE5/'>Page 5</a>
                    <a href='/deeper/../PAGE6/'>There and back page 6</a>
                    <a href='https://external.com/page7'>External Page 7</a>
                    <a>No Href Link</a>
                    <a href=' '>Empty Href Link</a>
                </body>
            </html>";

    [Fact]
    public void GetUrlsFromHtmlContent_ShouldExtractUrlsAndAppendBaseUrlCorrectly()
    {
        var expectedUrls = new[]
        {
            "http://test.com/page1",
            "https://test.com/page2/",
            "http://www.test.com/page3/",
            "http://test.com/page4//",
            "http://test.com/PAGE5/",
            "http://test.com/PAGE6/",
            "https://external.com/page7"
        };

        var actualUrls = _parser.GetUrlsFromHtmlContent(HtmlContent, BaseUrl).ToArray();
        actualUrls.ShouldBeEquivalentTo(expectedUrls);
    }
}