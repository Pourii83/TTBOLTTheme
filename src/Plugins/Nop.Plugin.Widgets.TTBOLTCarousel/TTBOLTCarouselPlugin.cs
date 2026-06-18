using System.Text.Json;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Infrastructure;

using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.TTBOLTCarousel;

public class TTBOLTCarouselPlugin : BasePlugin, IWidgetPlugin
{
    public bool HideInWidgetList => throw new NotImplementedException();

    public Type GetWidgetViewComponent(string widgetZone)
    {
        throw new NotImplementedException();
    }

    public Task<IList<string>> GetWidgetZonesAsync()
    {
        throw new NotImplementedException();
    }
}
