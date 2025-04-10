// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using WebCrawler.Abstraction;
using WebCrawler.Service;

if (args.Length != 1)
{
    Console.WriteLine("Usage: WebCrawler <startingUrl>");
    return;
}

var serviceProvider = ConfigureDepenencyInjection();

var crawler = serviceProvider.GetService<ICrawler>();
await crawler.RunAsync(args[0]);

ServiceProvider ConfigureDepenencyInjection()
{
    var services = new ServiceCollection();

    services.AddSingleton<ICrawler, CrawlerService>();
    services.AddSingleton<IUrlCollection, UrlCollection>();
    services.AddSingleton<IUrlParser, UrlParser>();
    services.AddHttpClient<IWebBrowser, WebBrowser>();

    var serviceProvider1 = services.BuildServiceProvider();
    return serviceProvider1;
}
