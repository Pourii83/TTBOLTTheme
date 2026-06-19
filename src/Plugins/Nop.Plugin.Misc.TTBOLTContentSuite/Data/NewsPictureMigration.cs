using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.News;
using Nop.Data;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.TTBOLTContentSuite.Domain;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Data;

[NopMigration("2026-06-18 00:00:02", "Misc.TTBOLTContentSuite 4.90.5. Add PictureId to news items", MigrationProcessType.Update)]
public class NewsPictureMigration : Migration
{
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var newsItemTableName = NameCompatibilityManager.GetTableName(typeof(NewsItem));
        if (!Schema.Table(newsItemTableName).Column(nameof(TTNewsItem.PictureId)).Exists())
            Alter.Table(newsItemTableName)
                .AddColumn(nameof(TTNewsItem.PictureId)).AsInt32().ForeignKey<Picture>(onDelete: Rule.Cascade).Nullable();
    }

    public override void Down()
    {
    }
}
