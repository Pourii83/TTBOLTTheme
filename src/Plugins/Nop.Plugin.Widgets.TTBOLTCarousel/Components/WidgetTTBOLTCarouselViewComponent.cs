using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.TTBOLTCarousel.Components;

public class WidgetTTBOLTCarouselViewComponent : NopViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        return View("~/Plugins/Widgets.TTBOLTCarousel/Views/PublicInfo.cshtml");
    }
}
