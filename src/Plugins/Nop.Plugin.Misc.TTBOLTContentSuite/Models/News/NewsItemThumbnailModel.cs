using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Models.News;

public record NewsItemThumbnailModel : BaseNopEntityModel
{
    [UIHint("Picture")]
    [NopResourceDisplayName("Plugins.Misc.TTBOLTContentSuite.NewsItem.Picture")]
    public int PictureId { get; set; }
}
