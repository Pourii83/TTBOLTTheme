using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.TTBOLTContentSuite.Domain;
using Nop.Plugin.Misc.TTBOLTContentSuite.Models.Blogs;
using Nop.Plugin.Misc.TTBOLTContentSuite.Models.News;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Models.News;
using AdminBlogPostModel = Nop.Web.Areas.Admin.Models.Blogs.BlogPostModel;
using AdminNewsItemModel = Nop.Web.Areas.Admin.Models.News.NewsItemModel;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Factories;

public class ContentSuiteModelFactory : IContentSuiteModelFactory
{
    private const int HomepagePictureSize = 520;
    private const string PictureIdFormKey = "PictureId";

    private readonly IRepository<TTBlogPost> _blogPostRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRepository<TTNewsItem> _newsItemRepository;
    private readonly IPictureService _pictureService;
    private readonly IStoreContext _storeContext;
    private readonly IStoreMappingService _storeMappingService;
    private readonly IUrlRecordService _urlRecordService;
    private readonly IWorkContext _workContext;

    public ContentSuiteModelFactory(
        IRepository<TTBlogPost> blogPostRepository,
        IHttpContextAccessor httpContextAccessor,
        IRepository<TTNewsItem> newsItemRepository,
        IPictureService pictureService,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IUrlRecordService urlRecordService,
        IWorkContext workContext)
    {
        _blogPostRepository = blogPostRepository;
        _httpContextAccessor = httpContextAccessor;
        _newsItemRepository = newsItemRepository;
        _pictureService = pictureService;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _urlRecordService = urlRecordService;
        _workContext = workContext;
    }

    public async Task<IList<HomePageBlogPostModel>> PrepareHomePageBlogPostModelsAsync()
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
            model.Add(new HomePageBlogPostModel
            {
                Title = blogPost.Title,
                BodyOverview = blogPost.BodyOverview,
                SeName = await _urlRecordService.GetSeNameAsync(blogPost, blogPost.LanguageId, ensureTwoPublishedLanguages: false),
                PictureUrl = await GetPictureUrlAsync(blogPost.PictureId)
            });
        }

        return model;
    }

    public async Task<IList<HomePageNewsItemModel>> PrepareHomePageNewsItemModelsAsync(HomepageNewsItemsModel newsItemsModel)
    {
        var model = new List<HomePageNewsItemModel>();
        foreach (var item in newsItemsModel.NewsItems)
        {
            var newsItem = await _newsItemRepository.GetByIdAsync(item.Id);
            model.Add(new HomePageNewsItemModel
            {
                Title = item.Title,
                Short = item.Short,
                SeName = item.SeName,
                PictureUrl = await GetPictureUrlAsync(newsItem?.PictureId)
            });
        }

        return model;
    }

    public async Task<BlogPostThumbnailModel> PrepareBlogPostThumbnailModelAsync(AdminBlogPostModel blogPostModel)
    {
        var pictureId = GetPostedPictureId();

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

    public async Task<NewsItemThumbnailModel> PrepareNewsItemThumbnailModelAsync(AdminNewsItemModel newsItemModel)
    {
        var pictureId = GetPostedPictureId();

        if (newsItemModel.Id > 0)
        {
            var newsItem = await _newsItemRepository.GetByIdAsync(newsItemModel.Id);
            pictureId = pictureId > 0 ? pictureId : newsItem?.PictureId ?? 0;
        }

        return new NewsItemThumbnailModel
        {
            Id = newsItemModel.Id,
            PictureId = pictureId
        };
    }

    private async Task<string> GetPictureUrlAsync(int? pictureId)
    {
        var pictureUrl = pictureId.HasValue && pictureId.Value > 0
            ? await _pictureService.GetPictureUrlAsync(pictureId.Value, HomepagePictureSize, showDefaultPicture: false)
            : string.Empty;

        return string.IsNullOrEmpty(pictureUrl)
            ? await _pictureService.GetDefaultPictureUrlAsync(HomepagePictureSize)
            : pictureUrl;
    }

    private int GetPostedPictureId()
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request?.HasFormContentType != true)
            return 0;

        if (!request.Form.TryGetValue(PictureIdFormKey, out var rawPictureId))
            return 0;

        return int.TryParse(rawPictureId.FirstOrDefault(), out var postedPictureId)
            ? postedPictureId
            : 0;
    }
}
