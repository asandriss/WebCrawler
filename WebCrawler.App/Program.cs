// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using WebCrawler.Abstraction;
using WebCrawler.Service;

if (args.Length != 1)
{
    Console.WriteLine("Usage: WebCrawler <startingUrl>");
    return;
}

var serviceProvider = ConfigureDepenencyInjection();

ConfigureLogger();

var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
var crawler = serviceProvider.GetService<ICrawler>();
var collection = serviceProvider.GetRequiredService<IUrlCollection>();

logger.LogInformation("Starting crawl at {Url}", args[0]);
await crawler!.RunAsync(args[0]);

ServiceProvider ConfigureDepenencyInjection()
{
    var services = new ServiceCollection();

    services.AddSingleton<ICrawler, CrawlerService>();
    services.AddSingleton<IUrlCollection, UrlCollection>();
    services.AddSingleton<IUrlParser, UrlParser>();
    services.AddHttpClient<IWebBrowser, WebBrowser>();

    services.AddLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddSerilog();
    });
    
    return services.BuildServiceProvider();
}

void ConfigureLogger()
{
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .MinimumLevel.Information()
        .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .CreateLogger();
}
