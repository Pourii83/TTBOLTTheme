using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Media;
using Nop.Data;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.TTBOLTContentSuite.Domain;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Data;
[NopSchemaMigration("2026-06-18 00:00:00", "Add PictureId to blog posts", MigrationProcessType.Installation)]

public class SchemaMigration : Migration
{
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var blogPostTableName = NameCompatibilityManager.GetTableName(typeof(BlogPost));
        if (!Schema.Table(blogPostTableName).Column(nameof(TTBlogPost.PictureId)).Exists())
            Alter.Table(blogPostTableName)
                .AddColumn(nameof(TTBlogPost.PictureId)).AsInt32().ForeignKey<Picture>(onDelete: Rule.Cascade).Nullable();
    }
    public override void Down()
    {
    }

}
