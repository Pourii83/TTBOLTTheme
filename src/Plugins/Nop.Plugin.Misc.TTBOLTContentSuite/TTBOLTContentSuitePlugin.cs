using Nop.Core.Domain.Cms;
using Nop.Plugin.Misc.TTBOLTContentSuite.Components;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Misc.TTBOLTContentSuite;

public class TTBOLTContentSuitePlugin : BasePlugin, IWidgetPlugin
{
    private readonly ILocalizationService _localizationService;
    private readonly ISettingService _settingService;
    private readonly WidgetSettings _widgetSettings;

    public TTBOLTContentSuitePlugin(
        ILocalizationService localizationService,
        ISettingService settingService,
        WidgetSettings widgetSettings)
    {
        _localizationService = localizationService;
        _settingService = settingService;
        _widgetSettings = widgetSettings;
    }

    public bool HideInWidgetList => false;

    public Type GetWidgetViewComponent(string widgetZone)
    {
        return typeof(TTBOLTContentSuiteViewComponent);
    }

    public Task<IList<string>> GetWidgetZonesAsync()
    {
        return Task.FromResult<IList<string>>(new List<string>
        {
            PublicWidgetZones.HomepageBeforeNews,
            AdminWidgetZones.BlogPostDetailsBlock,
            AdminWidgetZones.NewsItemsDetailsBlock
        });
    }

    public override async Task InstallAsync()
    {
        if (!_widgetSettings.ActiveWidgetSystemNames.Contains(PluginDescriptor.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(PluginDescriptor.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Misc.TTBOLTContentSuite.BlogPost.Thumbnail"] = "تصویر شاخص مقاله",
            ["Plugins.Misc.TTBOLTContentSuite.BlogPost.Picture"] = "تصویر شاخص",
            ["Plugins.Misc.TTBOLTContentSuite.NewsItem.Thumbnail"] = "تصویر شاخص خبر",
            ["Plugins.Misc.TTBOLTContentSuite.NewsItem.Picture"] = "تصویر شاخص",
            ["Plugins.Misc.TTBOLTContentSuite.HomePageBlogs.Title"] = "مقالات جدید",
            ["Plugins.Misc.TTBOLTContentSuite.HomePageBlogs.ReadMore"] = "بیشتر بدانید"
        });

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        if (_widgetSettings.ActiveWidgetSystemNames.Contains(PluginDescriptor.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Remove(PluginDescriptor.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.TTBOLTContentSuite");

        await base.UninstallAsync();
    }
}
