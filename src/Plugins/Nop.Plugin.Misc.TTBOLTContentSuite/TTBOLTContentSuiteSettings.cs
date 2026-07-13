using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.TTBOLTContentSuite;

public class TTBOLTContentSuiteSettings : ISettings
{
    public static readonly string[] SupportedHeadingTags =
    {
        "h1",
        "h2",
        "h3",
        "h4",
        "h5",
        "h6"
    };

    public List<string> TableOfContentsHeadingTags { get; set; } = new()
    {
        "h2",
        "h3"
    };

    public List<string> NewsTableOfContentsHeadingTags { get; set; } = new()
    {
        "h2",
        "h3"
    };
}
