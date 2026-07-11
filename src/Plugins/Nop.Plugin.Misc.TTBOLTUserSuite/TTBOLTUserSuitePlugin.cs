using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.TTBOLTUserSuite;

public class TTBOLTUserSuitePlugin : BasePlugin
{
    private readonly ILocalizationService _localizationService;
    private readonly ISettingService _settingService;

    public TTBOLTUserSuitePlugin(
        ILocalizationService localizationService,
        ISettingService settingService)
    {
        _localizationService = localizationService;
        _settingService = settingService;
    }

    public override async Task InstallAsync()
    {
        await _settingService.SaveSettingAsync(new TTBOLTUserSuiteSettings
        {
            Enabled = true,
            MaximumPictureSizeBytes = TTBOLTUserSuiteDefaults.DefaultMaximumPictureSizeBytes
        });

        await _localizationService.AddOrUpdateLocaleResourceAsync(
            TTBOLTUserSuiteDefaults.GetLocalizationResources());

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        await _settingService.DeleteSettingAsync<TTBOLTUserSuiteSettings>();
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.TTBOLTUserSuite");

        await base.UninstallAsync();
    }
}
