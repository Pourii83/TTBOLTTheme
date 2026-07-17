namespace Nop.Plugin.Widgets.TTBOLTCarousel.Utilities;

public static class PictureUrlFormatter
{
    public static string EncodePath(string pictureUrl)
    {
        if (string.IsNullOrWhiteSpace(pictureUrl))
            return pictureUrl;

        var fragment = string.Empty;
        var fragmentIndex = pictureUrl.IndexOf('#');
        if (fragmentIndex >= 0)
        {
            fragment = pictureUrl[fragmentIndex..];
            pictureUrl = pictureUrl[..fragmentIndex];
        }

        var query = string.Empty;
        var queryIndex = pictureUrl.IndexOf('?');
        if (queryIndex >= 0)
        {
            query = pictureUrl[queryIndex..];
            pictureUrl = pictureUrl[..queryIndex];
        }

        var pathStart = GetPathStartIndex(pictureUrl);
        var prefix = pictureUrl[..pathStart];
        var path = pictureUrl[pathStart..];

        if (string.IsNullOrEmpty(path))
            return string.Concat(prefix, query, fragment);

        var encodedPath = string.Join("/", path.Split('/').Select(EncodePathSegment));

        return string.Concat(prefix, encodedPath, query, fragment);
    }

    private static int GetPathStartIndex(string url)
    {
        if (url.StartsWith("//", StringComparison.Ordinal))
        {
            var pathStart = url.IndexOf('/', 2);
            return pathStart < 0 ? url.Length : pathStart;
        }

        var schemeSeparatorIndex = url.IndexOf("://", StringComparison.Ordinal);
        if (schemeSeparatorIndex >= 0)
        {
            var pathStart = url.IndexOf('/', schemeSeparatorIndex + 3);
            return pathStart < 0 ? url.Length : pathStart;
        }

        return 0;
    }

    private static string EncodePathSegment(string segment)
    {
        if (string.IsNullOrEmpty(segment))
            return segment;

        return Uri.EscapeDataString(Uri.UnescapeDataString(segment));
    }
}
