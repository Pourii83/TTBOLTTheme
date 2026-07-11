using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.TTBOLTUserSuite.Components;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Misc.TTBOLTUserSuite;

public class TTBOLTUserSuitePlugin : BasePlugin, IWidgetPlugin
{
    private readonly CustomerSettings _customerSettings;
    private readonly ILocalizationService _localizationService;
    private readonly ISettingService _settingService;
    private readonly WidgetSettings _widgetSettings;

    public TTBOLTUserSuitePlugin(
        CustomerSettings customerSettings,
        ILocalizationService localizationService,
        ISettingService settingService,
        WidgetSettings widgetSettings)
    {
        _customerSettings = customerSettings;
        _localizationService = localizationService;
        _settingService = settingService;
        _widgetSettings = widgetSettings;
    }

    public bool HideInWidgetList => false;

    public Type GetWidgetViewComponent(string widgetZone)
    {
        return typeof(TTBOLTUserSuiteViewComponent);
    }

    public Task<IList<string>> GetWidgetZonesAsync()
    {
        return Task.FromResult<IList<string>>(new List<string>
        {
            PublicWidgetZones.CustomerInfoTop
        });
    }

    public override async Task InstallAsync()
    {
        if (!_widgetSettings.ActiveWidgetSystemNames.Contains(PluginDescriptor.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(PluginDescriptor.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        if (!_customerSettings.AllowCustomersToUploadAvatars)
        {
            _customerSettings.AllowCustomersToUploadAvatars = true;
            await _settingService.SaveSettingAsync(_customerSettings);
        }

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Misc.TTBOLTUserSuite.ProfilePicture.Title"] = "عکس پروفایل",
            ["Plugins.Misc.TTBOLTUserSuite.ProfilePicture.Description"] = "عکسی برای حساب کاربری خود انتخاب کنید.",
            ["Plugins.Misc.TTBOLTUserSuite.ProfilePicture.Add"] = "افزودن عکس",
            ["Plugins.Misc.TTBOLTUserSuite.ProfilePicture.Change"] = "تغییر عکس"
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

        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.TTBOLTUserSuite");

        await base.UninstallAsync();
    }
}
