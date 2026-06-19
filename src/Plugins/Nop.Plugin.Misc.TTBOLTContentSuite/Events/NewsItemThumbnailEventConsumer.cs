using Microsoft.AspNetCore.Http;
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

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRepository<TTNewsItem> _newsItemRepository;

    public NewsItemThumbnailEventConsumer(
        IHttpContextAccessor httpContextAccessor,
        IRepository<TTNewsItem> newsItemRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _newsItemRepository = newsItemRepository;
    }

    public async Task HandleEventAsync(EntityInsertedEvent<NewsItem> eventMessage)
    {
        await SavePictureIdAsync(eventMessage.Entity.Id);
    }

    public async Task HandleEventAsync(EntityUpdatedEvent<NewsItem> eventMessage)
    {
        await SavePictureIdAsync(eventMessage.Entity.Id);
    }

    private async Task SavePictureIdAsync(int newsItemId)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null || !request.HasFormContentType)
            return;

        if (!request.Form.TryGetValue(PictureIdFormKey, out var rawPictureId))
            return;

        if (!int.TryParse(rawPictureId.FirstOrDefault(), out var pictureId))
            return;

        var newsItem = await _newsItemRepository.GetByIdAsync(newsItemId);
        if (newsItem == null)
            return;

        var normalizedPictureId = pictureId > 0 ? pictureId : (int?)null;
        if (newsItem.PictureId == normalizedPictureId)
            return;

        newsItem.PictureId = normalizedPictureId;
        await _newsItemRepository.UpdateAsync(newsItem);
    }
}
