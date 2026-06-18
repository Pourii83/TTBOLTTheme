using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Models.Blogs;

public record BlogPostThumbnailModel : BaseNopEntityModel
{
    [UIHint("Picture")]
    [NopResourceDisplayName("Plugins.Misc.TTBOLTContentSuite.BlogPost.Picture")]
    public int PictureId { get; set; }
}
