using JetBrains.Annotations;
using Shouldly;
using WebCrawler.Service;
using Xunit;

namespace WebCrawler.Service.Tests;

[TestSubject(typeof(HtmlParserExtension))]
public class HtmlParserExtensionTest
{
    [Theory]
    [InlineData("http://www.test.com/", "test.com")]
    [InlineData("https://www.test.com/", "test.com")]
    [InlineData("https://www.test.com/page", "test.com/page")]
    [InlineData("https://www.test.com/page/", "test.com/page")]
    [InlineData("/page", "test.com/page")]
    [InlineData("/PAGE", "test.com/page")]
    public void NormalizeUrl_CorrectUrl_ShouldRetrunNormalizedUrl(string input, string expected)
    {
        var actual = input.NormalizeUrl("http://test.com");
        
        actual.ShouldBe(expected);
    }
}