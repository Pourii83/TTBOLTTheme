using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.News;
using Nop.Data;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using Nop.Data.Migrations;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Data;
[NopSchemaMigration("2026-06-18 00:00:00", "Add PictureId to News", MigrationProcessType.Installation)]

public class SchemaMigration : Migration
{
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var blogPostTableName = NameCompatibilityManager.GetTableName(typeof(BlogPost));
        if (!Schema.Table(blogPostTableName).Column(nameof(Domain.BlogPost.PictureId)).Exists())
            Alter.Table(blogPostTableName)
                .AddColumn(nameof(Domain.BlogPost.PictureId)).AsInt32().ForeignKey<Picture>(onDelete: Rule.Cascade).Nullable();

        var newsItemTableName = NameCompatibilityManager.GetTableName(typeof(NewsItem));
        if (!Schema.Table(newsItemTableName).Column(nameof(Domain.NewsItem.PictureId)).Exists())
            Alter.Table(newsItemTableName)
                .AddColumn(nameof(Domain.NewsItem.PictureId)).AsInt32().ForeignKey<Picture>(onDelete: Rule.Cascade).Nullable();
    }
    public override void Down()
    {
    }

}
