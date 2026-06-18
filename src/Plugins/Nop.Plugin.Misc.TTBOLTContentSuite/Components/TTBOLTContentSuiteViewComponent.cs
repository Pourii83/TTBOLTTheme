using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.TTBOLTContentSuite.Domain;
using Nop.Plugin.Misc.TTBOLTContentSuite.Models.Blogs;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Models.Blogs;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Components;

public class TTBOLTContentSuiteViewComponent : NopViewComponent
{
    private const int HomepageBlogPostPictureSize = 520;
    private const string PictureIdFormKey = "PictureId";

    private readonly IRepository<TTBlogPost> _blogPostRepository;
    private readonly IPictureService _pictureService;
    private readonly IStoreMappingService _storeMappingService;
    private readonly IStoreContext _storeContext;
    private readonly IUrlRecordService _urlRecordService;
    private readonly IWorkContext _workContext;

    public TTBOLTContentSuiteViewComponent(
        IRepository<TTBlogPost> blogPostRepository,
        IPictureService pictureService,
        IStoreMappingService storeMappingService,
        IStoreContext storeContext,
        IUrlRecordService urlRecordService,
        IWorkContext workContext)
    {
        _blogPostRepository = blogPostRepository;
        _pictureService = pictureService;
        _storeMappingService = storeMappingService;
        _storeContext = storeContext;
        _urlRecordService = urlRecordService;
        _workContext = workContext;
    }

    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        if (widgetZone == PublicWidgetZones.HomepageBeforeNews)
        {
            var model = await PrepareHomePageBlogPostModelsAsync();
            if (!model.Any())
                return Content("");

            return View("~/Plugins/Misc.TTBOLTContentSuite/Views/Blog/HomePage.cshtml", model);
        }

        if (widgetZone == AdminWidgetZones.BlogPostDetailsBlock && additionalData is BlogPostModel blogPostModel)
            return View("~/Plugins/Misc.TTBOLTContentSuite/Views/Blog/AdminThumbnail.cshtml", await PrepareBlogPostThumbnailModelAsync(blogPostModel));

        return Content("");
    }

    private async Task<IList<HomePageBlogPostModel>> PrepareHomePageBlogPostModelsAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var language = await _workContext.GetWorkingLanguageAsync();
        var blogPosts = await _blogPostRepository.GetAllPagedAsync(async query =>
        {
            query = query.Where(blogPost => blogPost.LanguageId == language.Id);
            query = query.Where(blogPost => !blogPost.StartDateUtc.HasValue || blogPost.StartDateUtc <= DateTime.UtcNow);
            query = query.Where(blogPost => !blogPost.EndDateUtc.HasValue || blogPost.EndDateUtc >= DateTime.UtcNow);
            query = await _storeMappingService.ApplyStoreMapping(query, store.Id);

            return query.OrderByDescending(blogPost => blogPost.StartDateUtc ?? blogPost.CreatedOnUtc);
        }, pageSize: 4);

        var model = new List<HomePageBlogPostModel>();
        foreach (var blogPost in blogPosts)
        {
            var pictureUrl = blogPost.PictureId.HasValue && blogPost.PictureId.Value > 0
                ? await _pictureService.GetPictureUrlAsync(blogPost.PictureId.Value, HomepageBlogPostPictureSize, showDefaultPicture: false)
                : string.Empty;

            if (string.IsNullOrEmpty(pictureUrl))
                pictureUrl = await _pictureService.GetDefaultPictureUrlAsync(HomepageBlogPostPictureSize);

            model.Add(new HomePageBlogPostModel
            {
                Title = blogPost.Title,
                BodyOverview = blogPost.BodyOverview,
                SeName = await _urlRecordService.GetSeNameAsync(blogPost, blogPost.LanguageId, ensureTwoPublishedLanguages: false),
                PictureUrl = pictureUrl
            });
        }

        return model;
    }

    private async Task<BlogPostThumbnailModel> PrepareBlogPostThumbnailModelAsync(BlogPostModel blogPostModel)
    {
        var pictureId = 0;
        var request = HttpContext?.Request;
        if (request?.HasFormContentType == true &&
            request.Form.TryGetValue(PictureIdFormKey, out var rawPictureId) &&
            int.TryParse(rawPictureId.FirstOrDefault(), out var postedPictureId))
        {
            pictureId = postedPictureId;
        }

        if (blogPostModel.Id > 0)
        {
            var blogPost = await _blogPostRepository.GetByIdAsync(blogPostModel.Id);
            pictureId = pictureId > 0 ? pictureId : blogPost?.PictureId ?? 0;
        }

        return new BlogPostThumbnailModel
        {
            Id = blogPostModel.Id,
            PictureId = pictureId
        };
    }
}
