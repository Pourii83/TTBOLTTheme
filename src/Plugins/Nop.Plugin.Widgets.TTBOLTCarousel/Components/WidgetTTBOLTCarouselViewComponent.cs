using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widgets.TTBOLTCarousel.Areas.Admin.Services;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.TTBOLTCarousel.Components;

public class WidgetTTBOLTCarouselViewComponent : NopViewComponent
{
    private readonly ISliderItemService _sliderItemService;

    public WidgetTTBOLTCarouselViewComponent(ISliderItemService sliderItemService)
    {
        _sliderItemService = sliderItemService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        var slides = await _sliderItemService.GetSlideList();

        if (!slides.Any(slide => slide.PictureId.HasValue))
            return Content("");

        return View("~/Plugins/Widgets.TTBOLTCarousel/Views/PublicInfo.cshtml", slides);
    }
}
