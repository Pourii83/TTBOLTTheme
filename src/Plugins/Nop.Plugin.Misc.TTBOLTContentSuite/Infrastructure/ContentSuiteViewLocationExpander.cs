using Microsoft.AspNetCore.Mvc.Razor;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Infrastructure;

public class ContentSuiteViewLocationExpander : IViewLocationExpander
{
    public void PopulateValues(ViewLocationExpanderContext context)
    {
    }

    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        if (context.AreaName?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true)
            return viewLocations;

        if (context.ViewName.Equals("Components/HomepageNews/Default", StringComparison.OrdinalIgnoreCase))
        {
            return new[]
            {
                "/Plugins/Misc.TTBOLTContentSuite/Views/Shared/{0}.cshtml"
            }.Concat(viewLocations);
        }

        return viewLocations;
    }
}
