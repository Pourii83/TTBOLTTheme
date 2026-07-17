using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.News;
using Nop.Core.Events;
using Nop.Data;
using Nop.Plugin.Misc.TTBOLTContentSuite.Domain;
using Nop.Services.Events;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Events;

public class NewsItemThumbnailEventConsumer :
    IConsumer<EntityInsertedEvent<NewsItem>>,
    IConsumer<EntityUpdatedEvent<NewsItem>>
{
    private const string PictureIdFormKey = "PictureId";
    private const string ThumbnailPictureIdFormKey = "ThumbnailPictureId";

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRepository<TTNewsItem> _newsItemRepository;
    private readonly IWorkContext _workContext;

    public NewsItemThumbnailEventConsumer(
        IHttpContextAccessor httpContextAccessor,
        IRepository<TTNewsItem> newsItemRepository,
        IWorkContext workContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _newsItemRepository = newsItemRepository;
        _workContext = workContext;
    }

    public async Task HandleEventAsync(EntityInsertedEvent<NewsItem> eventMessage)
    {
        await SaveNewsItemMetadataAsync(eventMessage.Entity.Id, assignAuthor: true);
    }

    public async Task HandleEventAsync(EntityUpdatedEvent<NewsItem> eventMessage)
    {
        await SaveNewsItemMetadataAsync(eventMessage.Entity.Id, assignAuthor: false);
    }

    private async Task SaveNewsItemMetadataAsync(int newsItemId, bool assignAuthor)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null || !request.HasFormContentType)
            return;

        var newsItem = await _newsItemRepository.GetByIdAsync(newsItemId);
        if (newsItem == null)
            return;

        var pictureId = GetPostedPictureId(request, PictureIdFormKey);
        var thumbnailPictureId = GetPostedPictureId(request, ThumbnailPictureIdFormKey);
        newsItem.PictureId = pictureId > 0 ? pictureId : null;
        newsItem.ThumbnailPictureId = thumbnailPictureId > 0 ? thumbnailPictureId : null;

        if (assignAuthor || !newsItem.CustomerId.HasValue)
            newsItem.CustomerId = (await _workContext.GetCurrentCustomerAsync()).Id;

        if (!assignAuthor)
            newsItem.UpdatedOnUtc = DateTime.UtcNow;

        await _newsItemRepository.UpdateAsync(newsItem, publishEvent: false);
    }

    private static int GetPostedPictureId(HttpRequest request, string formKey)
    {
        return request.Form.TryGetValue(formKey, out var rawValue) &&
               int.TryParse(rawValue.FirstOrDefault(), out var pictureId)
            ? pictureId
            : 0;
    }
}
