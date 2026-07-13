using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Models;

public record ConfigurationModel : BaseNopModel
{
    public ConfigurationModel()
    {
        SelectedHeadingTags = new List<string>();
        AvailableHeadingTags = new List<SelectListItem>();
    }

    [NopResourceDisplayName("Plugins.Misc.TTBOLTContentSuite.Configuration.TableOfContentsHeadingTags")]
    public IList<string> SelectedHeadingTags { get; set; }

    public IList<SelectListItem> AvailableHeadingTags { get; set; }
}
