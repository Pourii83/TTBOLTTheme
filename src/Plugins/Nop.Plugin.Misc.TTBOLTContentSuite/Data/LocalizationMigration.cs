using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Data;

[NopMigration("2026-06-18 00:00:01", "Misc.TTBOLTContentSuite 4.90.5. Update localizations", MigrationProcessType.Update)]
public class LocalizationMigration : MigrationBase
{
    private readonly ILocalizationService _localizationService;

    public LocalizationMigration(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var (languageId, _) = this.GetLanguageData();

        _localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            ["Plugins.Misc.TTBOLTContentSuite.BlogPost.Thumbnail"] = "تصویر شاخص مقاله",
            ["Plugins.Misc.TTBOLTContentSuite.BlogPost.Picture"] = "تصویر شاخص",
            ["Plugins.Misc.TTBOLTContentSuite.HomePageBlogs.Title"] = "مقالات جدید",
            ["Plugins.Misc.TTBOLTContentSuite.HomePageBlogs.ReadMore"] = "بیشتر بدانید"
        }, languageId);
    }

    public override void Down()
    {
    }
}
