using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Widgets.TTBOLTCarousel.Domain;
using Nop.Services.Configuration;

namespace Nop.Plugin.Widgets.TTBOLTCarousel.Areas.Admin.Services;

public class SliderItemService : ISliderItemService
{
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;
    private readonly IWorkContext _workContext;

    public SliderItemService(ISettingService settingService,
        IStoreContext storeContext,
        IWorkContext workContext)
    {
        _settingService = settingService;
        _storeContext = storeContext;
        _workContext = workContext;
    }

    protected virtual async Task<int> GetActiveStoreScopeAsync()
    {
        return await _storeContext.GetActiveStoreScopeConfigurationAsync();
    }

    protected virtual async Task<List<SliderItem>> GetSlidesForStoreAsync(int storeId, bool loadSharedValueIfNotFound)
    {
        var key = $"{nameof(TTBOLTCarouselSettings)}.{nameof(TTBOLTCarouselSettings.Slides)}";
        var slidesSetting = await _settingService.GetSettingByKeyAsync(key, string.Empty, storeId: storeId, loadSharedValueIfNotFound: loadSharedValueIfNotFound);

        if (string.IsNullOrWhiteSpace(slidesSetting))
            return new List<SliderItem>();

        return JsonConvert.DeserializeObject<List<SliderItem>>(slidesSetting) ?? new List<SliderItem>();
    }

    protected virtual async Task SaveSlidesAsync(List<SliderItem> slides, int storeId)
    {
        var settings = await _settingService.LoadSettingAsync<TTBOLTCarouselSettings>(storeId);
        settings.Slides = JsonConvert.SerializeObject(slides.OrderBy(slide => slide.Order).ToList());

        await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.Slides, true, storeId);
    }

    public virtual async Task<SliderItem> GetSlideByIdAsync(int sliderItemId)
    {
        var storeId = await GetActiveStoreScopeAsync();
        var slides = await GetSlidesForStoreAsync(storeId, false);

        return slides.FirstOrDefault(slide => slide.Id == sliderItemId);
    }

    public virtual async Task<IPagedList<SliderItem>> GetAllSlides(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        if (storeId == 0)
            storeId = await GetActiveStoreScopeAsync();

        var slides = await GetSlidesForStoreAsync(storeId, false);
        var orderedSlides = slides.OrderBy(slide => slide.Order).ToList();

        return new PagedList<SliderItem>(orderedSlides, pageIndex, pageSize);
    }

    public virtual async Task InsertSlideAsync(SliderItem sliderItem)
    {
        var storeId = await GetActiveStoreScopeAsync();
        var slides = await GetSlidesForStoreAsync(storeId, false);

        sliderItem.Id = slides.Any() ? slides.Max(slide => slide.Id) + 1 : 1;
        slides.Add(sliderItem);

        await SaveSlidesAsync(slides, storeId);
    }

    public virtual async Task UpdateSlideAsync(SliderItem sliderItem)
    {
        var storeId = await GetActiveStoreScopeAsync();
        var slides = await GetSlidesForStoreAsync(storeId, false);
        var existingSlide = slides.FirstOrDefault(slide => slide.Id == sliderItem.Id);

        if (existingSlide == null)
            return;

        existingSlide.LanguageId = sliderItem.LanguageId;
        existingSlide.Order = sliderItem.Order;
        existingSlide.PictureId = sliderItem.PictureId;
        existingSlide.MobilePictureId = sliderItem.MobilePictureId;
        existingSlide.ImageAlt = sliderItem.ImageAlt;
        existingSlide.RouteLink = sliderItem.RouteLink;

        await SaveSlidesAsync(slides, storeId);
    }

    public virtual async Task DeleteSlideAsync(SliderItem sliderItem)
    {
        var storeId = await GetActiveStoreScopeAsync();
        var slides = await GetSlidesForStoreAsync(storeId, false);

        if (slides.RemoveAll(slide => slide.Id == sliderItem.Id) == 0)
            return;

        await SaveSlidesAsync(slides, storeId);
    }

    public async Task<List<SliderItem>> GetSlideList()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var language = await _workContext.GetWorkingLanguageAsync();
        var slides = await GetSlidesForStoreAsync(store.Id, true);

        return slides
            .Where(slide => slide.LanguageId == language.Id)
            .OrderBy(slide => slide.Order)
            .ToList();
    }
}
