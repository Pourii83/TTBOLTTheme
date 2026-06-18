using Nop.Services.Cms;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.TTBOLTContentSuite;

public class TTBOLTContentSuitePlugin : BasePlugin, IWidgetPlugin
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
