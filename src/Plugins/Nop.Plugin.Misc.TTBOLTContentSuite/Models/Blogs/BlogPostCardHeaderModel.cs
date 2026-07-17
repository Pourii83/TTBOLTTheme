using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Models.Blogs;

public record BlogPostCardHeaderModel : BaseNopEntityModel
{
    public string Title { get; set; }

    public string PictureUrl { get; set; }

    public string CustomerName { get; set; }

    public string CustomerAvatarUrl { get; set; }

    public DateTime CreatedOn { get; set; }
}
