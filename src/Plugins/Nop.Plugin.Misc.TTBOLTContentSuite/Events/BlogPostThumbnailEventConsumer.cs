using Microsoft.AspNetCore.Http;
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

    public BlogPostThumbnailEventConsumer(
        IRepository<TTBlogPost> blogPostRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _blogPostRepository = blogPostRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task HandleEventAsync(EntityInsertedEvent<BlogPost> eventMessage)
    {
        await SavePictureIdAsync(eventMessage.Entity.Id);
    }

    public async Task HandleEventAsync(EntityUpdatedEvent<BlogPost> eventMessage)
    {
        await SavePictureIdAsync(eventMessage.Entity.Id);
    }

    private async Task SavePictureIdAsync(int blogPostId)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null || !request.HasFormContentType)
            return;

        if (!request.Form.TryGetValue(PictureIdFormKey, out var rawPictureId))
            return;

        if (!int.TryParse(rawPictureId.FirstOrDefault(), out var pictureId))
            return;

        var blogPost = await _blogPostRepository.GetByIdAsync(blogPostId);
        if (blogPost == null)
            return;

        var normalizedPictureId = pictureId > 0 ? pictureId : (int?)null;
        if (blogPost.PictureId == normalizedPictureId)
            return;

        blogPost.PictureId = normalizedPictureId;
        await _blogPostRepository.UpdateAsync(blogPost);
    }
}
