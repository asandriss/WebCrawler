namespace WebCrawler.Service;

public static class HtmlHelperExtension
{
    public static string NormalizeUrl(this string url, string baseUrl)
    {
        var baseUri = new Uri(baseUrl);
        var absoluteUri = new Uri(baseUri, url);

        var normalizedHost = absoluteUri.Host.Replace("www.", "").ToLowerInvariant();
        var normalizedPath = absoluteUri.AbsolutePath.TrimEnd('/').ToLowerInvariant();

        return $"{normalizedHost}{normalizedPath}";
    }

    public static string NormalizeUri(this Uri uri)
    {
        var scheme = uri.Scheme.ToLowerInvariant();
        var host = uri.Host.Replace("www.", "").ToLowerInvariant();
        var path = uri.AbsolutePath.TrimEnd('/').ToLowerInvariant();

        // we could probably extend this to include more options, like ports/users/etc.
        return $"{scheme}://{host}{path}";
    }
}

