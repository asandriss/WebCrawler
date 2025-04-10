# WebCrawler

A lightweight, testable web crawler built in .NET using `HttpClient`, DI, and structured logging.  
Designed to traverse internal pages of a given website, collect links, and output a ranked list of all seen URLs.

---

## Features

- Internal link crawling only (same domain)
- Counts how many times each URL was seen
- Tracks both internal and external links
- Skips re-visiting pages (deduplication based on normalized URLs)
- Structured logging via `ILogger<T>` and Serilog
- Clean dependency injection with `HttpClientFactory`
- Fully unit tested (xUnit + Moq)

---

## Running the Crawler

Make sure .NET 7+ is installed.

```bash
dotnet run --project WebCrawler.ConsoleApp "https://example.com"
```

Output:
Ranked list of visited links (sorted by number of times specific link was referred to)

---

## Architecture Overview

### Project Structure

Solution consists of 3 top level projects

#### WebCrawler.Abstraction

This project does not contain any logic, it is only used as a dependency for other projects. It contains top level interfaces that define the structure of services

#### WebCrawler.Service

This is the main "back end" of the application. It contains the main services that make up the application.

- Crawler Service - this is the main "engine" of the application. It contains the main loop of the crawler
- WebBrowser Service - fetches raw HTML from an URL
- UrlParser - extracts links from raw html
- UrlCollection - tracks pending, visited and seen URLs

### External Libraries Used

- [Angle Sharp](https://github.com/AngleSharp/AngleSharp) - This library is used to Parse HTML and query anchor tags.
- [Serilog] - Used for strucutred logging. Currently only Conosle sink is used to for outputting text to console but other sinks can easily be added
- [XUnit], [Moq] and [Shouldly] - Testing frameworks.

---

## Testing

All core components are covered by unit tests.

Running the tests:

```
dotnet test
```

---

## Further Improvments (if more time allowed)

- Enforce max crawl depth and/or max pages to check (`MaxDepth` & `MaxPages` configurable params)
- Add crawl delays to reduce load on the web site (maybe add `Task.Delay(1000)` between each web request)
- Check and Respect `robots.txt` (honor `disallow` paths, limit user-agent)
- Support parallel crawling (eg. `Channel<T>` and `Task.WhenAll`)
- Clasify and retry failed HTTP requests
- Grouping of errors (timeouts, DNS, 404, etc)
- Add functional tests agains a mock website (for example with Python web server)
- Containerize the application with Docker
- Improve output (allow export to CSV, use GraphWiz to visualize reference paths)
