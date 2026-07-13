using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Data;
using Nop.Plugin.Misc.TTBOLTContentSuite.Domain;
using Nop.Plugin.Misc.TTBOLTContentSuite.Models.Blogs;
using Nop.Plugin.Misc.TTBOLTContentSuite.Models.News;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Models.News;
using AdminBlogPostModel = Nop.Web.Areas.Admin.Models.Blogs.BlogPostModel;
using AdminNewsItemModel = Nop.Web.Areas.Admin.Models.News.NewsItemModel;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Factories;

public class ContentSuiteModelFactory : IContentSuiteModelFactory
{
    private const int HomepageItemsCount = 4;
    private const int HomepagePictureSize = 520;
    private const int BlogCardPictureSize = 640;
    private const string PictureIdFormKey = "PictureId";

    private readonly IRepository<TTBlogPost> _blogPostRepository;
    private readonly CustomerSettings _customerSettings;
    private readonly ICustomerService _customerService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRepository<TTNewsItem> _newsItemRepository;
    private readonly MediaSettings _mediaSettings;
    private readonly IPictureService _pictureService;
    private readonly IStoreContext _storeContext;
    private readonly IStoreMappingService _storeMappingService;
    private readonly IUrlRecordService _urlRecordService;
    private readonly IWorkContext _workContext;

    public ContentSuiteModelFactory(
        IRepository<TTBlogPost> blogPostRepository,
        CustomerSettings customerSettings,
        ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        IHttpContextAccessor httpContextAccessor,
        IRepository<TTNewsItem> newsItemRepository,
        MediaSettings mediaSettings,
        IPictureService pictureService,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IUrlRecordService urlRecordService,
        IWorkContext workContext)
    {
        _blogPostRepository = blogPostRepository;
        _customerSettings = customerSettings;
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _httpContextAccessor = httpContextAccessor;
        _newsItemRepository = newsItemRepository;
        _mediaSettings = mediaSettings;
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
        }, pageSize: HomepageItemsCount);

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
        foreach (var item in newsItemsModel.NewsItems.Take(HomepageItemsCount))
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

    public async Task<BlogPostCardHeaderModel> PrepareBlogPostCardHeaderModelAsync(Nop.Web.Models.Blogs.BlogPostModel blogPostModel)
    {
        var blogPost = await _blogPostRepository.GetByIdAsync(blogPostModel.Id);
        var customer = blogPost?.CustomerId is > 0
            ? await _customerService.GetCustomerByIdAsync(blogPost.CustomerId.Value)
            : null;
        var customerName = customer == null
            ? string.Empty
            : await _customerService.GetCustomerFullNameAsync(customer);

        if (string.IsNullOrWhiteSpace(customerName))
            customerName = await _customerService.FormatUsernameAsync(customer);

        return new BlogPostCardHeaderModel
        {
            Id = blogPostModel.Id,
            Title = blogPostModel.Title,
            PictureUrl = await GetPictureUrlAsync(blogPost?.PictureId, BlogCardPictureSize),
            CustomerName = customerName,
            CustomerAvatarUrl = await GetCustomerAvatarUrlAsync(customer),
            CreatedOn = blogPostModel.CreatedOn
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

    private async Task<string> GetPictureUrlAsync(int? pictureId, int pictureSize = HomepagePictureSize)
    {
        var pictureUrl = pictureId.HasValue && pictureId.Value > 0
            ? await _pictureService.GetPictureUrlAsync(pictureId.Value, pictureSize, showDefaultPicture: false)
            : string.Empty;

        return string.IsNullOrEmpty(pictureUrl)
            ? await _pictureService.GetDefaultPictureUrlAsync(pictureSize)
            : pictureUrl;
    }

    private async Task<string> GetCustomerAvatarUrlAsync(Customer customer)
    {
        if (customer == null || !_customerSettings.AllowCustomersToUploadAvatars)
            return string.Empty;

        var avatarPictureId = await _genericAttributeService.GetAttributeAsync<int>(
            customer,
            NopCustomerDefaults.AvatarPictureIdAttribute);

        return await _pictureService.GetPictureUrlAsync(
            avatarPictureId,
            _mediaSettings.AvatarPictureSize,
            _customerSettings.DefaultAvatarEnabled,
            defaultPictureType: PictureType.Avatar);
    }

    private int GetPostedPictureId()
    {
        return GetPostedInt(PictureIdFormKey);
    }

    private int GetPostedInt(string formKey)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request?.HasFormContentType != true || !request.Form.TryGetValue(formKey, out var rawValue))
            return 0;

        return int.TryParse(rawValue.FirstOrDefault(), out var postedValue) ? postedValue : 0;
    }

}
