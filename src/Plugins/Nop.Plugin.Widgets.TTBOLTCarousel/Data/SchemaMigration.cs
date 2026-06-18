using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Media;
using Nop.Data;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.TTBOLTCarousel.Domain;

namespace Nop.Plugin.Widgets.TTBOLTCarousel.Data;

[NopSchemaMigration("2025-06-16 00:00:00", "Add SliderItem table", MigrationProcessType.Installation)]

public class SchemaMigration : Migration
{
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(SliderItem))).Exists())
        {
            Create.TableFor<SliderItem>();
        }
        else
        {
            var sliderItemTableName = NameCompatibilityManager.GetTableName(typeof(SliderItem));
            if (!Schema.Table(sliderItemTableName).Column(nameof(SliderItem.MobilePictureId)).Exists())
                Alter.Table(sliderItemTableName)
                    .AddColumn(nameof(SliderItem.MobilePictureId)).AsInt32().ForeignKey<Picture>(onDelete: Rule.Cascade).Nullable();
        }

    }
    public override void Down()
    {
       
    }
}
