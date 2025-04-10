using JetBrains.Annotations;
using Shouldly;
using Xunit;

namespace WebCrawler.Service.Tests;

[TestSubject(typeof(HtmlHelperExtension))]
public class HtmlHelperExtensionTest
{
    [Theory]
    [InlineData("http://www.test.com/", "test.com")]
    [InlineData("https://www.test.com/", "test.com")]
    [InlineData("https://www.test.com/page", "test.com/page")]
    [InlineData("https://www.test.com/page/", "test.com/page")]
    [InlineData("https://www.test.com/page//", "test.com/page")]
    [InlineData("/page", "test.com/page")]
    [InlineData("/PAGE", "test.com/page")]
    public void NormalizeUrl_CorrectUrl_ShouldRetrunNormalizedUrl(string input, string expected)
    {
        var baseUrl = "http://test.com";
        var actual = input.NormalizeUrl(baseUrl);
        
        actual.ShouldBe(expected);
    }
}