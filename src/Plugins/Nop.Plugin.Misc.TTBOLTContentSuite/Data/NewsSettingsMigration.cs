using FluentMigrator;
using Nop.Core.Domain.News;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Data;

[NopMigration("2026-06-18 00:00:03", "Misc.TTBOLTContentSuite 4.90.5. Set homepage news count", MigrationProcessType.Update)]
public class NewsSettingsMigration : Migration
{
    private readonly ISettingService _settingService;

    public NewsSettingsMigration(ISettingService settingService)
    {
        _settingService = settingService;
    }

    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var newsSettings = _settingService.LoadSetting<NewsSettings>();
        newsSettings.MainPageNewsCount = 4;
        _settingService.SaveSetting(newsSettings, settings => settings.MainPageNewsCount);
    }

    public override void Down()
    {
    }
}
