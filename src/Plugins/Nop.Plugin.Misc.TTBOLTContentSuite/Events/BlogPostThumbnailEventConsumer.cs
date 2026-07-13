using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Events;
using Nop.Data;
using Nop.Plugin.Misc.TTBOLTContentSuite.Domain;
using Nop.Plugin.Misc.TTBOLTContentSuite.Services;
using Nop.Services.Events;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Events;

public class BlogPostThumbnailEventConsumer :
    IConsumer<EntityInsertedEvent<BlogPost>>,
    IConsumer<EntityUpdatedEvent<BlogPost>>,
    IConsumer<EntityDeletedEvent<BlogPost>>
{
    private const string PictureIdFormKey = "PictureId";
    private const string ThumbnailPictureIdFormKey = "ThumbnailPictureId";
    private const string RelatedBlogPostIdsFormKey = "SelectedRelatedBlogPostIds";

    private readonly IRepository<TTBlogPost> _blogPostRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRelatedBlogPostService _relatedBlogPostService;
    private readonly IWorkContext _workContext;

    public BlogPostThumbnailEventConsumer(
        IRepository<TTBlogPost> blogPostRepository,
        IHttpContextAccessor httpContextAccessor,
        IRelatedBlogPostService relatedBlogPostService,
        IWorkContext workContext)
    {
        _blogPostRepository = blogPostRepository;
        _httpContextAccessor = httpContextAccessor;
        _relatedBlogPostService = relatedBlogPostService;
        _workContext = workContext;
    }

    public async Task HandleEventAsync(EntityInsertedEvent<BlogPost> eventMessage)
    {
        await SaveBlogPostMetadataAsync(eventMessage.Entity.Id, assignAuthor: true);
    }

    public async Task HandleEventAsync(EntityUpdatedEvent<BlogPost> eventMessage)
    {
        await SaveBlogPostMetadataAsync(eventMessage.Entity.Id, assignAuthor: false);
    }

    public async Task HandleEventAsync(EntityDeletedEvent<BlogPost> eventMessage)
    {
        await _relatedBlogPostService.DeleteRelatedBlogPostsAsync(eventMessage.Entity.Id);
    }

    private async Task SaveBlogPostMetadataAsync(int blogPostId, bool assignAuthor)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null || !request.HasFormContentType)
            return;

        var blogPost = await _blogPostRepository.GetByIdAsync(blogPostId);
        if (blogPost == null)
            return;

        var pictureId = GetPostedPictureId(request, PictureIdFormKey);
        var thumbnailPictureId = GetPostedPictureId(request, ThumbnailPictureIdFormKey);
        blogPost.PictureId = pictureId > 0 ? pictureId : null;
        blogPost.ThumbnailPictureId = thumbnailPictureId > 0 ? thumbnailPictureId : null;

        if (assignAuthor)
            blogPost.CustomerId = (await _workContext.GetCurrentCustomerAsync()).Id;

        await _blogPostRepository.UpdateAsync(blogPost, publishEvent: false);

        if (request.Form.ContainsKey(RelatedBlogPostIdsFormKey))
        {
            await _relatedBlogPostService.ReplaceRelatedBlogPostsAsync(
                blogPostId,
                GetPostedIds(request, RelatedBlogPostIdsFormKey));
        }
    }

    private static int GetPostedPictureId(HttpRequest request, string formKey)
    {
        return request.Form.TryGetValue(formKey, out var rawValue) &&
               int.TryParse(rawValue.FirstOrDefault(), out var pictureId)
            ? pictureId
            : 0;
    }

    private static IList<int> GetPostedIds(HttpRequest request, string formKey)
    {
        if (!request.Form.TryGetValue(formKey, out var rawValues))
            return new List<int>();

        return rawValues
            .Select(rawValue => int.TryParse(rawValue, out var value) ? value : 0)
            .Where(value => value > 0)
            .Distinct()
            .ToList();
    }
}
