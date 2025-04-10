using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
using Shouldly;
using Xunit;

namespace WebCrawler.Service.Tests;

[TestSubject(typeof(WebBrowser))]
public class WebBrowserTest
{
    [Fact]
    public async Task GetPageHtml_ValidUrl_ShouldReturnCorrectContent()
    {
        var expect = "<html>Page content</html>";
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expect)
        };
        var httpClient = CreateMockHttpClient(responseMessage);
        var browser = new WebBrowser(httpClient, NullLogger<WebBrowser>.Instance);

        var actual = await browser.GetPageHtml("http://unit.test");

        actual.ShouldNotBeNull();
        actual.ShouldBe(expect);
    }

    private HttpClient CreateMockHttpClient(HttpResponseMessage message)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(message);

        return new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://unit.test")
        };
    }
}