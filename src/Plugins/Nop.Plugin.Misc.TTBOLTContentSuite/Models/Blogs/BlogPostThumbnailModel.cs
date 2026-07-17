using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Models.Blogs;

public record BlogPostThumbnailModel : BaseNopEntityModel
{
    public BlogPostThumbnailModel()
    {
        SelectedRelatedBlogPostIds = new List<int>();
        AvailableRelatedBlogPosts = new List<SelectListItem>();
    }

    [UIHint("Picture")]
    [NopResourceDisplayName("Plugins.Misc.TTBOLTContentSuite.BlogPost.Picture")]
    public int PictureId { get; set; }

    [UIHint("Picture")]
    [NopResourceDisplayName("Plugins.Misc.TTBOLTContentSuite.BlogPost.ThumbnailPicture")]
    public int ThumbnailPictureId { get; set; }

    [NopResourceDisplayName("Plugins.Misc.TTBOLTContentSuite.BlogPost.RelatedPosts")]
    public IList<int> SelectedRelatedBlogPostIds { get; set; }

    public IList<SelectListItem> AvailableRelatedBlogPosts { get; set; }
}
