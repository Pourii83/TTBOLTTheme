using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.News;
using Nop.Data;
using Nop.Plugin.Misc.TTBOLTContentSuite.Domain;
using Nop.Plugin.Misc.TTBOLTContentSuite.Models;
using Nop.Plugin.Misc.TTBOLTContentSuite.Models.Blogs;
using Nop.Plugin.Misc.TTBOLTContentSuite.Models.News;
using Nop.Plugin.Misc.TTBOLTContentSuite.Services;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Models.News;
using AdminBlogPostModel = Nop.Web.Areas.Admin.Models.Blogs.BlogPostModel;
using AdminNewsItemModel = Nop.Web.Areas.Admin.Models.News.NewsItemModel;
using PublicBlogPostModel = Nop.Web.Models.Blogs.BlogPostModel;
using PublicNewsItemModel = Nop.Web.Models.News.NewsItemModel;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Factories;

public class ContentSuiteModelFactory : IContentSuiteModelFactory
{
    private const int HomepageItemsCount = 4;
    private const int HomepagePictureSize = 520;
    private const int BlogCardPictureSize = 640;
    private const int ContentPictureSize = 1200;
    private const int SidebarBlogPostItemsCount = 4;
    private const int SidebarBlogPostPictureSize = 240;
    private const int SidebarNewsItemsCount = 4;
    private const int SidebarNewsPictureSize = 240;
    private const string PictureIdFormKey = "PictureId";
    private const string ThumbnailPictureIdFormKey = "ThumbnailPictureId";
    private const string RelatedBlogPostIdsFormKey = "SelectedRelatedBlogPostIds";

    private readonly IRepository<TTBlogPost> _blogPostRepository;
    private readonly CustomerSettings _customerSettings;
    private readonly ICustomerService _customerService;
    private readonly IDateTimeHelper _dateTimeHelper;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRepository<TTNewsItem> _newsItemRepository;
    private readonly MediaSettings _mediaSettings;
    private readonly IPictureService _pictureService;
    private readonly IRelatedBlogPostService _relatedBlogPostService;
    private readonly IStoreContext _storeContext;
    private readonly IStoreMappingService _storeMappingService;
    private readonly TTBOLTContentSuiteSettings _contentSuiteSettings;
    private readonly IUrlRecordService _urlRecordService;
    private readonly IWorkContext _workContext;

    public ContentSuiteModelFactory(
        IRepository<TTBlogPost> blogPostRepository,
        CustomerSettings customerSettings,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IGenericAttributeService genericAttributeService,
        IHttpContextAccessor httpContextAccessor,
        IRepository<TTNewsItem> newsItemRepository,
        MediaSettings mediaSettings,
        IPictureService pictureService,
        IRelatedBlogPostService relatedBlogPostService,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        TTBOLTContentSuiteSettings contentSuiteSettings,
        IUrlRecordService urlRecordService,
        IWorkContext workContext)
    {
        _blogPostRepository = blogPostRepository;
        _customerSettings = customerSettings;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _genericAttributeService = genericAttributeService;
        _httpContextAccessor = httpContextAccessor;
        _newsItemRepository = newsItemRepository;
        _mediaSettings = mediaSettings;
        _pictureService = pictureService;
        _relatedBlogPostService = relatedBlogPostService;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _contentSuiteSettings = contentSuiteSettings;
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
                SeName = await _urlRecordService.GetSeNameAsync(
                    blogPost.Id,
                    nameof(BlogPost),
                    blogPost.LanguageId,
                    ensureTwoPublishedLanguages: false),
                PictureUrl = await GetPictureUrlAsync(blogPost.ThumbnailPictureId ?? blogPost.PictureId)
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
                PictureUrl = await GetPictureUrlAsync(newsItem?.ThumbnailPictureId ?? newsItem?.PictureId)
            });
        }

        return model;
    }

    public async Task<BlogPostThumbnailModel> PrepareBlogPostThumbnailModelAsync(AdminBlogPostModel blogPostModel)
    {
        var pictureId = GetPostedPictureId();
        var thumbnailPictureId = GetPostedInt(ThumbnailPictureIdFormKey);
        var hasPostedForm = _httpContextAccessor.HttpContext?.Request.HasFormContentType == true;
        var selectedRelatedBlogPostIds = hasPostedForm
            ? GetPostedIntList(RelatedBlogPostIdsFormKey)
            : new List<int>();

        if (blogPostModel.Id > 0)
        {
            var blogPost = await _blogPostRepository.GetByIdAsync(blogPostModel.Id);
            pictureId = pictureId > 0 ? pictureId : blogPost?.PictureId ?? 0;
            thumbnailPictureId = thumbnailPictureId > 0 ? thumbnailPictureId : blogPost?.ThumbnailPictureId ?? 0;

            if (!hasPostedForm)
            {
                selectedRelatedBlogPostIds = (await _relatedBlogPostService
                    .GetRelatedBlogPostsAsync(blogPostModel.Id))
                    .Select(mapping => mapping.RelatedBlogPostId)
                    .ToList();
            }
        }

        var model = new BlogPostThumbnailModel
        {
            Id = blogPostModel.Id,
            PictureId = pictureId,
            ThumbnailPictureId = thumbnailPictureId,
            SelectedRelatedBlogPostIds = selectedRelatedBlogPostIds
        };

        var blogPosts = await _blogPostRepository.GetAllAsync(query => query
            .Where(blogPost => blogPost.Id != blogPostModel.Id)
            .OrderByDescending(blogPost => blogPost.StartDateUtc ?? blogPost.CreatedOnUtc));

        foreach (var blogPost in blogPosts)
        {
            model.AvailableRelatedBlogPosts.Add(new SelectListItem
            {
                Text = blogPost.Title,
                Value = blogPost.Id.ToString()
            });
        }

        return model;
    }

    public async Task<BlogPostCardHeaderModel> PrepareBlogPostCardHeaderModelAsync(Nop.Web.Models.Blogs.BlogPostModel blogPostModel)
    {
        var blogPost = await _blogPostRepository.GetByIdAsync(blogPostModel.Id);
        var customer = blogPost?.CustomerId is > 0
            ? await _customerService.GetCustomerByIdAsync(blogPost.CustomerId.Value)
            : null;
        return new BlogPostCardHeaderModel
        {
            Id = blogPostModel.Id,
            Title = blogPostModel.Title,
            PictureUrl = await GetPictureUrlAsync(blogPost?.ThumbnailPictureId ?? blogPost?.PictureId, BlogCardPictureSize),
            CustomerName = await GetCustomerNameAsync(customer),
            CustomerAvatarUrl = await GetCustomerAvatarUrlAsync(customer),
            CreatedOn = blogPostModel.CreatedOn
        };
    }

    public async Task<BlogPostContentModel> PrepareBlogPostContentModelAsync(PublicBlogPostModel blogPostModel)
    {
        var blogPost = await _blogPostRepository.GetByIdAsync(blogPostModel.Id);
        var customer = blogPost?.CustomerId is > 0
            ? await _customerService.GetCustomerByIdAsync(blogPost.CustomerId.Value)
            : null;
        var selectedHeadingTags = (_contentSuiteSettings.TableOfContentsHeadingTags ?? new List<string>())
            .Select(headingTag => headingTag?.Trim().ToLowerInvariant())
            .Where(headingTag => TTBOLTContentSuiteSettings.SupportedHeadingTags.Contains(headingTag))
            .Distinct()
            .ToHashSet();

        return new BlogPostContentModel
        {
            Id = blogPostModel.Id,
            PictureUrl = await GetOptionalPictureUrlAsync(blogPost?.PictureId, ContentPictureSize),
            Title = blogPostModel.Title,
            CustomerName = await GetCustomerNameAsync(customer),
            CustomerAvatarUrl = await GetCustomerAvatarUrlAsync(customer),
            CreatedOn = blogPostModel.CreatedOn,
            UpdatedOn = blogPost?.UpdatedOnUtc is DateTime updatedOnUtc
                ? await _dateTimeHelper.ConvertToUserTimeAsync(updatedOnUtc, DateTimeKind.Utc)
                : null,
            HeadingTags = TTBOLTContentSuiteSettings.SupportedHeadingTags
                .Where(selectedHeadingTags.Contains)
                .ToList()
        };
    }

    public async Task<BlogPostSidebarModel> PrepareBlogPostSidebarModelAsync(PublicBlogPostModel blogPostModel)
    {
        var mappings = await _relatedBlogPostService.GetRelatedBlogPostsAsync(blogPostModel.Id);
        var relatedBlogPostIds = mappings.Select(mapping => mapping.RelatedBlogPostId).ToList();
        var store = await _storeContext.GetCurrentStoreAsync();
        var language = await _workContext.GetWorkingLanguageAsync();
        var blogPosts = await _blogPostRepository.GetAllAsync(async query =>
        {
            query = query.Where(blogPost => blogPost.Id != blogPostModel.Id);
            query = query.Where(blogPost => blogPost.LanguageId == language.Id);
            query = query.Where(blogPost => !blogPost.StartDateUtc.HasValue || blogPost.StartDateUtc <= DateTime.UtcNow);
            query = query.Where(blogPost => !blogPost.EndDateUtc.HasValue || blogPost.EndDateUtc >= DateTime.UtcNow);

            return await _storeMappingService.ApplyStoreMapping(query, store.Id);
        });

        var blogPostsById = blogPosts.ToDictionary(blogPost => blogPost.Id);
        var model = new BlogPostSidebarModel();

        foreach (var mapping in mappings)
        {
            if (!blogPostsById.TryGetValue(mapping.RelatedBlogPostId, out var blogPost))
                continue;

            model.RelatedPosts.Add(await PrepareBlogPostSidebarItemModelAsync(blogPost));

            if (model.RelatedPosts.Count == SidebarBlogPostItemsCount)
                break;
        }

        var randomBlogPosts = blogPosts
            .Where(blogPost => !relatedBlogPostIds.Contains(blogPost.Id))
            .OrderBy(_ => Random.Shared.Next())
            .Take(SidebarBlogPostItemsCount);

        foreach (var blogPost in randomBlogPosts)
            model.RandomPosts.Add(await PrepareBlogPostSidebarItemModelAsync(blogPost));

        return model;
    }

    public async Task<NewsItemThumbnailModel> PrepareNewsItemThumbnailModelAsync(AdminNewsItemModel newsItemModel)
    {
        var pictureId = GetPostedPictureId();
        var thumbnailPictureId = GetPostedInt(ThumbnailPictureIdFormKey);

        if (newsItemModel.Id > 0)
        {
            var newsItem = await _newsItemRepository.GetByIdAsync(newsItemModel.Id);
            pictureId = pictureId > 0 ? pictureId : newsItem?.PictureId ?? 0;
            thumbnailPictureId = thumbnailPictureId > 0 ? thumbnailPictureId : newsItem?.ThumbnailPictureId ?? 0;
        }

        return new NewsItemThumbnailModel
        {
            Id = newsItemModel.Id,
            PictureId = pictureId,
            ThumbnailPictureId = thumbnailPictureId
        };
    }

    public async Task<NewsItemContentModel> PrepareNewsItemContentModelAsync(PublicNewsItemModel newsItemModel)
    {
        var newsItem = await _newsItemRepository.GetByIdAsync(newsItemModel.Id);
        var customer = newsItem?.CustomerId is > 0
            ? await _customerService.GetCustomerByIdAsync(newsItem.CustomerId.Value)
            : null;
        var selectedHeadingTags = (_contentSuiteSettings.NewsTableOfContentsHeadingTags ?? new List<string>())
            .Select(headingTag => headingTag?.Trim().ToLowerInvariant())
            .Where(headingTag => TTBOLTContentSuiteSettings.SupportedHeadingTags.Contains(headingTag))
            .Distinct()
            .ToHashSet();

        return new NewsItemContentModel
        {
            Id = newsItemModel.Id,
            PictureUrl = await GetOptionalPictureUrlAsync(newsItem?.PictureId, ContentPictureSize),
            Title = newsItemModel.Title,
            CustomerName = await GetCustomerNameAsync(customer),
            CustomerAvatarUrl = await GetCustomerAvatarUrlAsync(customer),
            CreatedOn = newsItemModel.CreatedOn,
            UpdatedOn = newsItem?.UpdatedOnUtc is DateTime updatedOnUtc
                ? await _dateTimeHelper.ConvertToUserTimeAsync(updatedOnUtc, DateTimeKind.Utc)
                : null,
            HeadingTags = TTBOLTContentSuiteSettings.SupportedHeadingTags
                .Where(selectedHeadingTags.Contains)
                .ToList()
        };
    }

    public async Task<NewsItemCardHeaderModel> PrepareNewsItemCardHeaderModelAsync(PublicNewsItemModel newsItemModel)
    {
        var newsItem = await _newsItemRepository.GetByIdAsync(newsItemModel.Id);
        var customer = newsItem?.CustomerId is > 0
            ? await _customerService.GetCustomerByIdAsync(newsItem.CustomerId.Value)
            : null;

        return new NewsItemCardHeaderModel
        {
            Id = newsItemModel.Id,
            PictureUrl = await GetPictureUrlAsync(newsItem?.ThumbnailPictureId ?? newsItem?.PictureId, BlogCardPictureSize),
            Title = newsItemModel.Title,
            SeName = newsItemModel.SeName,
            CustomerName = await GetCustomerNameAsync(customer),
            CustomerAvatarUrl = await GetCustomerAvatarUrlAsync(customer),
            CreatedOn = newsItemModel.CreatedOn,
            UpdatedOn = newsItem?.UpdatedOnUtc is DateTime updatedOnUtc
                ? await _dateTimeHelper.ConvertToUserTimeAsync(updatedOnUtc, DateTimeKind.Utc)
                : null
        };
    }

    public async Task<IList<NewsItemSidebarItemModel>> PrepareRandomNewsItemModelsAsync(int currentNewsItemId)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var language = await _workContext.GetWorkingLanguageAsync();
        var newsItems = await _newsItemRepository.GetAllAsync(async query =>
        {
            query = query.Where(newsItem => newsItem.Id != currentNewsItemId);
            query = query.Where(newsItem => newsItem.Published);
            query = query.Where(newsItem => newsItem.LanguageId == language.Id);
            query = query.Where(newsItem => !newsItem.StartDateUtc.HasValue || newsItem.StartDateUtc <= DateTime.UtcNow);
            query = query.Where(newsItem => !newsItem.EndDateUtc.HasValue || newsItem.EndDateUtc >= DateTime.UtcNow);

            return await _storeMappingService.ApplyStoreMapping(query, store.Id);
        });

        var model = new List<NewsItemSidebarItemModel>();
        foreach (var newsItem in newsItems.OrderBy(_ => Random.Shared.Next()).Take(SidebarNewsItemsCount))
        {
            model.Add(new NewsItemSidebarItemModel
            {
                Title = newsItem.Title,
                SeName = await _urlRecordService.GetSeNameAsync(
                    newsItem.Id,
                    nameof(NewsItem),
                    newsItem.LanguageId,
                    ensureTwoPublishedLanguages: false),
                PictureUrl = await GetPictureUrlAsync(
                    newsItem.ThumbnailPictureId ?? newsItem.PictureId,
                    SidebarNewsPictureSize),
                CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(
                    newsItem.CreatedOnUtc,
                    DateTimeKind.Utc)
            });
        }

        return model;
    }

    private async Task<BlogPostSidebarItemModel> PrepareBlogPostSidebarItemModelAsync(TTBlogPost blogPost)
    {
        return new BlogPostSidebarItemModel
        {
            Title = blogPost.Title,
            SeName = await _urlRecordService.GetSeNameAsync(
                blogPost.Id,
                nameof(BlogPost),
                blogPost.LanguageId,
                ensureTwoPublishedLanguages: false),
            PictureUrl = await GetPictureUrlAsync(
                blogPost.ThumbnailPictureId ?? blogPost.PictureId,
                SidebarBlogPostPictureSize),
            CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(blogPost.CreatedOnUtc, DateTimeKind.Utc)
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

    private async Task<string> GetCustomerNameAsync(Customer customer)
    {
        if (customer == null)
            return string.Empty;

        var customerName = await _customerService.GetCustomerFullNameAsync(customer);
        return string.IsNullOrWhiteSpace(customerName)
            ? await _customerService.FormatUsernameAsync(customer)
            : customerName;
    }

    private async Task<string> GetOptionalPictureUrlAsync(int? pictureId, int pictureSize)
    {
        return pictureId is > 0
            ? await _pictureService.GetPictureUrlAsync(pictureId.Value, pictureSize, showDefaultPicture: false)
            : string.Empty;
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

    private List<int> GetPostedIntList(string formKey)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request?.HasFormContentType != true || !request.Form.TryGetValue(formKey, out var rawValues))
            return new List<int>();

        return rawValues
            .Select(rawValue => int.TryParse(rawValue, out var value) ? value : 0)
            .Where(value => value > 0)
            .Distinct()
            .ToList();
    }

}
