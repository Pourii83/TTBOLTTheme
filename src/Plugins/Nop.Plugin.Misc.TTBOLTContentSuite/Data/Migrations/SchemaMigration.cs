using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.News;
using Nop.Data;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.TTBOLTContentSuite.Domain;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Data.Migrations;

[NopSchemaMigration("2026-07-12 01:00:00", "Misc.TTBOLTContentSuite 4.90.9 schema", MigrationProcessType.Installation)]

public class SchemaMigration : MigrationBase
{
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;
        //BlogPost
        var blogPostTableName = NameCompatibilityManager.GetTableName(typeof(BlogPost));
        if (!Schema.Table(blogPostTableName).Column(nameof(TTBlogPost.PictureId)).Exists())
            Alter.Table(blogPostTableName)
                .AddColumn(nameof(TTBlogPost.PictureId)).AsInt32().ForeignKey<Picture>(onDelete: Rule.Cascade).Nullable();

        if (!Schema.Table(blogPostTableName).Column(nameof(TTBlogPost.CustomerId)).Exists())
            Alter.Table(blogPostTableName)
                .AddColumn(nameof(TTBlogPost.CustomerId)).AsInt32().ForeignKey<Customer>(onDelete: Rule.None).Nullable();
        //NewsItem
        var newsItemTableName = NameCompatibilityManager.GetTableName(typeof(NewsItem));
        if (!Schema.Table(newsItemTableName).Column(nameof(TTNewsItem.PictureId)).Exists())
            Alter.Table(newsItemTableName)
                .AddColumn(nameof(TTNewsItem.PictureId)).AsInt32().ForeignKey<Picture>(onDelete: Rule.Cascade).Nullable();

    }

    public override void Down()
    {
    }
}
