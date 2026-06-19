using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.TTBOLTContentSuite.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
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

        if (widgetZone == AdminWidgetZones.BlogPostDetailsBlock && additionalData is AdminBlogPostModel blogPostModel)
            return View("~/Plugins/Misc.TTBOLTContentSuite/Views/Blog/AdminThumbnail.cshtml",
                await _contentSuiteModelFactory.PrepareBlogPostThumbnailModelAsync(blogPostModel));

        if (widgetZone == AdminWidgetZones.NewsItemsDetailsBlock && additionalData is AdminNewsItemModel newsItemModel)
            return View("~/Plugins/Misc.TTBOLTContentSuite/Views/News/AdminThumbnail.cshtml",
                await _contentSuiteModelFactory.PrepareNewsItemThumbnailModelAsync(newsItemModel));

        return Content("");
    }
}
