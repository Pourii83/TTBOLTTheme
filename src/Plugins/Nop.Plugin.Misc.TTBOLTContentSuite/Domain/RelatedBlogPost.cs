using Nop.Core;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Domain;

public class RelatedBlogPost : BaseEntity
{
    public int BlogPostId { get; set; }

    public int RelatedBlogPostId { get; set; }

    public int DisplayOrder { get; set; }
}
