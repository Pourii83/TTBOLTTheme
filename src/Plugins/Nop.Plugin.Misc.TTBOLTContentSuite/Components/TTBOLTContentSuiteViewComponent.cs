using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Components;
public class TTBOLTContentSuiteViewComponent : NopViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        if (widgetZone == PublicWidgetZones.HomepageBeforeNews)
        {
            return View("~/Plugins/Misc.TTBOLTContentSuite/Views/Blog/HomePage.cshtml");
        }
        else
            return Content("");
    }
}
