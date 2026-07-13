namespace Nop.Plugin.Misc.TTBOLTContentSuite.Models.Blogs;

public record BlogPostSidebarModel
{
    public BlogPostSidebarModel()
    {
        RelatedPosts = new List<BlogPostSidebarItemModel>();
        RandomPosts = new List<BlogPostSidebarItemModel>();
    }

    public IList<BlogPostSidebarItemModel> RelatedPosts { get; set; }

    public IList<BlogPostSidebarItemModel> RandomPosts { get; set; }
}
