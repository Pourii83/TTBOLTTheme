using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Plugin.Widgets.TTBOLTCarousel.Components;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.TTBOLTCarousel;

public class TTBOLTCarouselPlugin : BasePlugin, IWidgetPlugin
{
    private readonly ILocalizationService _localizationService;
    private readonly ISettingService _settingService;
    private readonly IWebHelper _webHelper;
    private readonly WidgetSettings _widgetSettings;

    public TTBOLTCarouselPlugin(ILocalizationService localizationService,
        ISettingService settingService,
        IWebHelper webHelper,
        WidgetSettings widgetSettings)
    {
        _localizationService = localizationService;
        _settingService = settingService;
        _webHelper = webHelper;
        _widgetSettings = widgetSettings;
    }

    public Task<IList<string>> GetWidgetZonesAsync()
    {
        return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.HomepageTop });
    }

    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/TTBOLTCarousel/Configure";
    }

    public Type GetWidgetViewComponent(string widgetZone)
    {
        return typeof(WidgetTTBOLTCarouselViewComponent);
    }

    public override async Task InstallAsync()
    {
        await _settingService.SaveSettingAsync(new TTBOLTCarouselSettings
        {
            Slides = "[]"
        });

        if (!_widgetSettings.ActiveWidgetSystemNames.Contains(PluginDescriptor.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(PluginDescriptor.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Widgets.TTBOLTCarousel.Fields.Picture"] = "Picture",
            ["Plugins.Widgets.TTBOLTCarousel.Fields.MobilePicture"] = "Mobile picture",
            ["Plugins.Widgets.TTBOLTCarousel.Fields.ImageAlt"] = "Image alternate text",
            ["Plugins.Widgets.TTBOLTCarousel.Fields.Language"] = "Language",
            ["Plugins.Widgets.TTBOLTCarousel.Fields.RouteLink"] = "Route link",
            ["Plugins.Widgets.TTBOLTCarousel.Fields.Order"] = "Display order",
            ["Plugins.Widgets.TTBOLTCarousel.Fields.MobilePicture"] = "Mobile picture",
            ["Plugins.Widgets.TTBOLTCarousel.Fields.PictureUrl"] = "Picture URL",
        });

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        await _settingService.DeleteSettingAsync<TTBOLTCarouselSettings>();

        if (_widgetSettings.ActiveWidgetSystemNames.Contains(PluginDescriptor.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Remove(PluginDescriptor.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Widgets.TTBOLTCarousel");

        await base.UninstallAsync();
    }

    public bool HideInWidgetList => false;
}
