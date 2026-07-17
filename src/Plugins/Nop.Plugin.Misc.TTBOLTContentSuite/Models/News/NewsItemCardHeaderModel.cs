using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Models.News;

public record NewsItemCardHeaderModel : BaseNopEntityModel
{
    public string Title { get; set; }

    public string SeName { get; set; }

    public string PictureUrl { get; set; }

    public string CustomerName { get; set; }

    public string CustomerAvatarUrl { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? UpdatedOn { get; set; }
}
