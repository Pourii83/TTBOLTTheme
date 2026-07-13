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

[NopSchemaMigration("2026-07-13 20:00:00", "Misc.TTBOLTContentSuite 4.90.10 schema", MigrationProcessType.Installation)]

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

        if (!Schema.Table(blogPostTableName).Column(nameof(TTBlogPost.ThumbnailPictureId)).Exists())
            Alter.Table(blogPostTableName)
                .AddColumn(nameof(TTBlogPost.ThumbnailPictureId)).AsInt32().Nullable();

        if (!Schema.Table(blogPostTableName).Column(nameof(TTBlogPost.CustomerId)).Exists())
            Alter.Table(blogPostTableName)
                .AddColumn(nameof(TTBlogPost.CustomerId)).AsInt32().ForeignKey<Customer>(onDelete: Rule.None).Nullable();

        if (!Schema.Table(blogPostTableName).Column(nameof(TTBlogPost.UpdatedOnUtc)).Exists())
            Alter.Table(blogPostTableName)
                .AddColumn(nameof(TTBlogPost.UpdatedOnUtc)).AsDateTime2().Nullable();
        //NewsItem
        var newsItemTableName = NameCompatibilityManager.GetTableName(typeof(NewsItem));
        if (!Schema.Table(newsItemTableName).Column(nameof(TTNewsItem.PictureId)).Exists())
            Alter.Table(newsItemTableName)
                .AddColumn(nameof(TTNewsItem.PictureId)).AsInt32().ForeignKey<Picture>(onDelete: Rule.Cascade).Nullable();

        if (!Schema.Table(newsItemTableName).Column(nameof(TTNewsItem.ThumbnailPictureId)).Exists())
            Alter.Table(newsItemTableName)
                .AddColumn(nameof(TTNewsItem.ThumbnailPictureId)).AsInt32().Nullable();

        var relatedBlogPostTableName = NameCompatibilityManager.GetTableName(typeof(RelatedBlogPost));
        if (!Schema.Table(relatedBlogPostTableName).Exists())
            Create.TableFor<RelatedBlogPost>();

    }

    public override void Down()
    {
    }
}

[NopMigration("2026-07-13 21:00:00", "Misc.TTBOLTContentSuite 4.90.12 related blog posts", MigrationProcessType.Update)]
public class RelatedBlogPostSchemaMigration : MigrationBase
{
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var tableName = NameCompatibilityManager.GetTableName(typeof(RelatedBlogPost));
        if (!Schema.Table(tableName).Exists())
            Create.TableFor<RelatedBlogPost>();
    }

    public override void Down()
    {
    }
}

[NopMigration("2026-07-14 09:00:00", "Misc.TTBOLTContentSuite 4.90.15 blog post updated date", MigrationProcessType.Update)]
public class BlogPostUpdatedDateSchemaMigration : MigrationBase
{
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var blogPostTableName = NameCompatibilityManager.GetTableName(typeof(BlogPost));
        if (!Schema.Table(blogPostTableName).Column(nameof(TTBlogPost.UpdatedOnUtc)).Exists())
            Alter.Table(blogPostTableName)
                .AddColumn(nameof(TTBlogPost.UpdatedOnUtc)).AsDateTime2().Nullable();
    }

    public override void Down()
    {
    }
}
