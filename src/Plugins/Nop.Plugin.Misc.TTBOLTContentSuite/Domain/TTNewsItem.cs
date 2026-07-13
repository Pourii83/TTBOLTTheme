using Nop.Core.Domain.News;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Domain;

public class TTNewsItem : NewsItem
{
    public int? PictureId { get; set; }

    public int? ThumbnailPictureId { get; set; }
}
