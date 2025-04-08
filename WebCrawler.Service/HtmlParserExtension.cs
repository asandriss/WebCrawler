namespace WebCrawler.Service;

public static class HtmlParserExtension
{
    public static string NormalizeUrl(this string url, string baseUrl)
    {
        var baseUri = new Uri(baseUrl);
        var absoluteUri = new Uri(baseUri, url);

        var normalizedHost = absoluteUri.Host.Replace("www.", "").ToLowerInvariant();
        var normalizedPath = absoluteUri.AbsolutePath.TrimEnd('/').ToLowerInvariant();

        return $"{normalizedHost}{normalizedPath}";
    }
}