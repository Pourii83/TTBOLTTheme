using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widgets.TTBOLTCarousel.Areas.Admin.Services;
using Nop.Plugin.Widgets.TTBOLTCarousel.Model;
using Nop.Services.Media;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.TTBOLTCarousel.Controllers;

public class TTBOLTCarouselController : BasePluginController
{
    private readonly IPictureService _pictureService;
    private readonly ISliderItemService _sliderItemService;

    public TTBOLTCarouselController(IPictureService pictureService,
        ISliderItemService sliderItemService)
    {
        _pictureService = pictureService;
        _sliderItemService = sliderItemService;
    }

    public async Task<IActionResult> GetClientPictures([FromBody] WindowModel window)
    {
        var isMobile = window.Width < 910;
        var sliderItems = new List<SliderItemModel>();

        foreach (var slide in await _sliderItemService.GetSlideList())
        {
            if (!slide.PictureId.HasValue)
                continue;

            sliderItems.Add(new SliderItemModel
            {
                RouteLink = slide.RouteLink,
                ImageAlt = slide.ImageAlt,
                PictureUrl = isMobile
                    ? await _pictureService.GetPictureUrlAsync(slide.MobilePictureId ?? slide.PictureId.Value)
                    : await _pictureService.GetPictureUrlAsync(slide.PictureId.Value),
                Order = slide.Order
            });
        }

        return Json(sliderItems);
    }
}
