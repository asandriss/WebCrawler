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
var collection = serviceProvider.GetRequiredService<IUrlCollection>();

await crawler.RunAsync(args[0]);

Console.WriteLine($"*** Ranked List of Links under {args[0]} ***");

foreach (var (url, count) in collection
             .GetAllSeenCounts()
             .OrderByDescending(x => x.Count)
             .ThenBy(x => x.Url))
{
    Console.WriteLine($"{count}x\t{url}");
}

ServiceProvider ConfigureDepenencyInjection()
{
    var services = new ServiceCollection();

    services.AddSingleton<ICrawler, CrawlerService>();
    services.AddSingleton<IUrlCollection, UrlCollection>();
    services.AddSingleton<IUrlParser, UrlParser>();
    services.AddHttpClient<IWebBrowser, WebBrowser>();

    return services.BuildServiceProvider();
}
