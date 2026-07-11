using FluentMigrator;
using Nop.Core.Domain.Cms;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;
using Nop.Services.Localization;

namespace Nop.Plugin.Misc.TTBOLTUserSuite.Data;

[NopMigration("2026-07-11 12:00:00", "Misc.TTBOLTUserSuite 4.90.2. Rebuild profile picture feature", MigrationProcessType.Update)]
public class UpgradeTo4902Migration : Migration
{
    private readonly ILocalizationService _localizationService;
    private readonly ISettingService _settingService;

    public UpgradeTo4902Migration(
        ILocalizationService localizationService,
        ISettingService settingService)
    {
        _localizationService = localizationService;
        _settingService = settingService;
    }

    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        _settingService.SaveSetting(new TTBOLTUserSuiteSettings
        {
            Enabled = true,
            MaximumPictureSizeBytes = TTBOLTUserSuiteDefaults.DefaultMaximumPictureSizeBytes
        });

        var widgetSettings = _settingService.LoadSetting<WidgetSettings>();
        if (widgetSettings.ActiveWidgetSystemNames.Remove("Misc.TTBOLTUserSuite"))
            _settingService.SaveSetting(widgetSettings);

        _localizationService.AddOrUpdateLocaleResource(
            TTBOLTUserSuiteDefaults.GetLocalizationResources());
    }

    public override void Down()
    {
    }
}
