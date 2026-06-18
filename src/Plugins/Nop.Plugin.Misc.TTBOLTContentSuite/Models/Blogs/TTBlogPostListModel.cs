using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Blogs;

public partial record TTBlogPostListModel : BaseNopModel
{
    public TTBlogPostListModel()
    {
        PagingFilteringContext = new BlogPagingFilteringModel();
        BlogPosts = new List<TTBlogPostModel>();
    }

    public int WorkingLanguageId { get; set; }
    public BlogPagingFilteringModel PagingFilteringContext { get; set; }
    public IList<TTBlogPostModel> BlogPosts { get; set; }
}