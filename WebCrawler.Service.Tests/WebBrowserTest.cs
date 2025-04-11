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

    [Fact]
    public async Task GetPageHtml_NotFound_ShouldReturnNull()
    {
        var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
        var httpClient = CreateMockHttpClient(responseMessage);
        var browser = new WebBrowser(httpClient, NullLogger<WebBrowser>.Instance);

        var actual = await browser.GetPageHtml("http://unit.test");

        actual.ShouldBeNull();
    }

    [Fact]
    public async Task GetPageHtml_ServerError_ShouldReturnNull()
    {
        var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        var httpClient = CreateMockHttpClient(responseMessage);
        var browser = new WebBrowser(httpClient, NullLogger<WebBrowser>.Instance);

        var actual = await browser.GetPageHtml("http://unit.test");

        actual.ShouldBeNull();
    }

    [Fact]
    public async Task GetPageHtml_Timeout_ShouldReturnNull()
    {
        var timeOutInSeconds = 2;
        var handlerMock = new Mock<HttpMessageHandler>();
        
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Returns<HttpRequestMessage, CancellationToken>(async (_, _) =>
            {
                await Task.Delay(TimeSpan.FromSeconds(timeOutInSeconds + 1));
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://unit.test")
        };
        var browser = new WebBrowser(httpClient, NullLogger<WebBrowser>.Instance);

        var actual = await browser.GetPageHtml("http://unit.test", timeOutInSeconds);

        actual.ShouldBeNull();
    }

    [Fact]
    public async Task GetPageHtml_HttpRequestException_ShouldReturnNull()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Network error"));

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://unit.test")
        };
        var browser = new WebBrowser(httpClient, NullLogger<WebBrowser>.Instance);

        var actual = await browser.GetPageHtml("http://unit.test");

        actual.ShouldBeNull();
    }
}