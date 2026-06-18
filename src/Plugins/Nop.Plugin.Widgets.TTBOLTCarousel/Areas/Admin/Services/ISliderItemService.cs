using Nop.Core;
using Nop.Plugin.Widgets.TTBOLTCarousel.Domain;

namespace Nop.Plugin.Widgets.TTBOLTCarousel.Areas.Admin.Services;
public interface ISliderItemService
{
    Task<IPagedList<SliderItem>> GetAllSlides(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue);
    Task<List<SliderItem>> GetSlideList();
    Task<SliderItem> GetSlideByIdAsync(int sliderItemId);
    Task InsertSlideAsync(SliderItem sliderItem);
    Task UpdateSlideAsync(SliderItem slidesliderItemItem);
    Task DeleteSlideAsync(SliderItem sliderItem);

    void Log(SliderItem sliderItem);
}
