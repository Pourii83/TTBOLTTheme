using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Events;
using Nop.Data;
using Nop.Plugin.Misc.TTBOLTContentSuite.Domain;
using Nop.Services.Events;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Events;

public class BlogPostThumbnailEventConsumer :
    IConsumer<EntityInsertedEvent<BlogPost>>,
    IConsumer<EntityUpdatedEvent<BlogPost>>
{
    private const string PictureIdFormKey = "PictureId";

    private readonly IRepository<TTBlogPost> _blogPostRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWorkContext _workContext;

    public BlogPostThumbnailEventConsumer(
        IRepository<TTBlogPost> blogPostRepository,
        IHttpContextAccessor httpContextAccessor,
        IWorkContext workContext)
    {
        _blogPostRepository = blogPostRepository;
        _httpContextAccessor = httpContextAccessor;
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

    private async Task SaveBlogPostMetadataAsync(int blogPostId, bool assignAuthor)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null || !request.HasFormContentType)
            return;

        var blogPost = await _blogPostRepository.GetByIdAsync(blogPostId);
        if (blogPost == null)
            return;

        var pictureId = GetPostedPictureId(request, PictureIdFormKey);
        blogPost.PictureId = pictureId > 0 ? pictureId : null;

        if (assignAuthor)
            blogPost.CustomerId = (await _workContext.GetCurrentCustomerAsync()).Id;

        await _blogPostRepository.UpdateAsync(blogPost);
    }

    private static int GetPostedPictureId(HttpRequest request, string formKey)
    {
        return request.Form.TryGetValue(formKey, out var rawValue) &&
               int.TryParse(rawValue.FirstOrDefault(), out var pictureId)
            ? pictureId
            : 0;
    }
}
