using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.TTBOLTContentSuite.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using PublicBlogPostModel = Nop.Web.Models.Blogs.BlogPostModel;
using PublicNewsItemModel = Nop.Web.Models.News.NewsItemModel;
using AdminBlogPostModel = Nop.Web.Areas.Admin.Models.Blogs.BlogPostModel;
using AdminNewsItemModel = Nop.Web.Areas.Admin.Models.News.NewsItemModel;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Components;

public class TTBOLTContentSuiteViewComponent : NopViewComponent
{
    private readonly IContentSuiteModelFactory _contentSuiteModelFactory;

    public TTBOLTContentSuiteViewComponent(IContentSuiteModelFactory contentSuiteModelFactory)
    {
        _contentSuiteModelFactory = contentSuiteModelFactory;
    }

    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        if (widgetZone == PublicWidgetZones.HomepageBeforeNews)
        {
            var model = await _contentSuiteModelFactory.PrepareHomePageBlogPostModelsAsync();
            if (!model.Any())
                return Content("");

            return View("~/Plugins/Misc.TTBOLTContentSuite/Views/Blog/HomePage.cshtml", model);
        }

        if (widgetZone == PublicWidgetZones.BlogListPageBeforePost && additionalData is PublicBlogPostModel publicBlogPostModel)
            return View("~/Plugins/Misc.TTBOLTContentSuite/Views/Blog/CardHeader.cshtml",
                await _contentSuiteModelFactory.PrepareBlogPostCardHeaderModelAsync(publicBlogPostModel));

        if (widgetZone == PublicWidgetZones.LeftSideColumnBlogBefore && additionalData is PublicBlogPostModel sidebarBlogPostModel)
        {
            var model = await _contentSuiteModelFactory.PrepareBlogPostSidebarModelAsync(sidebarBlogPostModel);
            return model.RelatedPosts.Count == 0 && model.RandomPosts.Count == 0
                ? Content("")
                : View("~/Plugins/Misc.TTBOLTContentSuite/Views/Blog/BlogPostSidebar.cshtml", model);
        }

        if (widgetZone == PublicWidgetZones.BlogPostPageBeforeBody && additionalData is PublicBlogPostModel blogPostDetailsModel)
        {
            var model = await _contentSuiteModelFactory.PrepareBlogPostContentPictureModelAsync(blogPostDetailsModel);
            return string.IsNullOrEmpty(model.PictureUrl)
                ? Content("")
                : View("~/Plugins/Misc.TTBOLTContentSuite/Views/Shared/ContentPicture.cshtml", model);
        }

        if (widgetZone == PublicWidgetZones.NewsItemPageBeforeBody && additionalData is PublicNewsItemModel newsItemDetailsModel)
        {
            var model = await _contentSuiteModelFactory.PrepareNewsItemContentPictureModelAsync(newsItemDetailsModel);
            return string.IsNullOrEmpty(model.PictureUrl)
                ? Content("")
                : View("~/Plugins/Misc.TTBOLTContentSuite/Views/Shared/ContentPicture.cshtml", model);
        }

        if (widgetZone == PublicWidgetZones.NewsListPageInsideItem && additionalData is PublicNewsItemModel publicNewsItemModel)
            return View("~/Plugins/Misc.TTBOLTContentSuite/Views/News/ListThumbnail.cshtml",
                await _contentSuiteModelFactory.PrepareNewsListThumbnailModelAsync(publicNewsItemModel));

        if (widgetZone == AdminWidgetZones.BlogPostDetailsBlock && additionalData is AdminBlogPostModel blogPostModel)
            return View("~/Plugins/Misc.TTBOLTContentSuite/Views/Blog/AdminThumbnail.cshtml",
                await _contentSuiteModelFactory.PrepareBlogPostThumbnailModelAsync(blogPostModel));

        if (widgetZone == AdminWidgetZones.NewsItemsDetailsBlock && additionalData is AdminNewsItemModel newsItemModel)
            return View("~/Plugins/Misc.TTBOLTContentSuite/Views/News/AdminThumbnail.cshtml",
                await _contentSuiteModelFactory.PrepareNewsItemThumbnailModelAsync(newsItemModel));

        return Content("");
    }
}
