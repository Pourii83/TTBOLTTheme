using Nop.Core.Domain.Blogs;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Domain;
public class TTBlogPost : BlogPost
{
    public int? PictureId { get; set; }

    public int? ThumbnailPictureId { get; set; }

    public int? CustomerId { get; set; }
}
