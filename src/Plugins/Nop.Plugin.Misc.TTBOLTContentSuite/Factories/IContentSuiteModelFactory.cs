using Nop.Plugin.Misc.TTBOLTContentSuite.Models;
using Nop.Plugin.Misc.TTBOLTContentSuite.Models.Blogs;
using Nop.Plugin.Misc.TTBOLTContentSuite.Models.News;
using Nop.Web.Models.News;
using AdminBlogPostModel = Nop.Web.Areas.Admin.Models.Blogs.BlogPostModel;
using AdminNewsItemModel = Nop.Web.Areas.Admin.Models.News.NewsItemModel;
using PublicBlogPostModel = Nop.Web.Models.Blogs.BlogPostModel;
using PublicNewsItemModel = Nop.Web.Models.News.NewsItemModel;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Factories;

public interface IContentSuiteModelFactory
{
    Task<IList<HomePageBlogPostModel>> PrepareHomePageBlogPostModelsAsync();

    Task<IList<HomePageNewsItemModel>> PrepareHomePageNewsItemModelsAsync(HomepageNewsItemsModel newsItemsModel);

    Task<BlogPostThumbnailModel> PrepareBlogPostThumbnailModelAsync(AdminBlogPostModel blogPostModel);

    Task<BlogPostCardHeaderModel> PrepareBlogPostCardHeaderModelAsync(Nop.Web.Models.Blogs.BlogPostModel blogPostModel);

    Task<BlogPostContentModel> PrepareBlogPostContentModelAsync(PublicBlogPostModel blogPostModel);

    Task<BlogPostSidebarModel> PrepareBlogPostSidebarModelAsync(PublicBlogPostModel blogPostModel);

    Task<NewsItemThumbnailModel> PrepareNewsItemThumbnailModelAsync(AdminNewsItemModel newsItemModel);

    Task<NewsItemContentModel> PrepareNewsItemContentModelAsync(PublicNewsItemModel newsItemModel);

    Task<NewsItemCardHeaderModel> PrepareNewsItemCardHeaderModelAsync(PublicNewsItemModel newsItemModel);

    Task<IList<NewsItemSidebarItemModel>> PrepareRandomNewsItemModelsAsync(int currentNewsItemId);
}
