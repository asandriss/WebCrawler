using JetBrains.Annotations;
using Shouldly;
using WebCrawler.Service;
using Xunit;

namespace WebCrawler.Service.Tests;

[TestSubject(typeof(CrawlerService))]
public class CrawlerServiceTest
{
    // [Fact]
    // public void EchoMe_NewString_ShouldEcho()
    // {
    //     var sut = new CrawlerService();
    //     var expect = "Echo me!";
    //
    //     var actual = sut.EchoMe(expect);
    //     actual.ShouldBe(expect);
    // }
}