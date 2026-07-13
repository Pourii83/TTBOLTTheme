namespace Nop.Plugin.Misc.TTBOLTContentSuite.Models.Blogs;

public record BlogPostSidebarItemModel
{
    public string Title { get; set; }

    public string SeName { get; set; }

    public string PictureUrl { get; set; }

    public DateTime CreatedOn { get; set; }
}
