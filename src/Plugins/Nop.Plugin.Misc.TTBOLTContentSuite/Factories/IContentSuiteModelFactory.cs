using Nop.Plugin.Misc.TTBOLTContentSuite.Models.Blogs;
using Nop.Plugin.Misc.TTBOLTContentSuite.Models.News;
using Nop.Web.Models.News;
using AdminBlogPostModel = Nop.Web.Areas.Admin.Models.Blogs.BlogPostModel;
using AdminNewsItemModel = Nop.Web.Areas.Admin.Models.News.NewsItemModel;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Factories;

public interface IContentSuiteModelFactory
{
    Task<IList<HomePageBlogPostModel>> PrepareHomePageBlogPostModelsAsync();

    Task<IList<HomePageNewsItemModel>> PrepareHomePageNewsItemModelsAsync(HomepageNewsItemsModel newsItemsModel);

    Task<BlogPostThumbnailModel> PrepareBlogPostThumbnailModelAsync(AdminBlogPostModel blogPostModel);

    Task<NewsItemThumbnailModel> PrepareNewsItemThumbnailModelAsync(AdminNewsItemModel newsItemModel);
}
