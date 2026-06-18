using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Plugin.Widgets.TTBOLTCarousel.Areas.Admin.Services;
using Nop.Plugin.Widgets.TTBOLTCarousel.Model;
using Nop.Services.Media;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.PW.HomeSlider.Controllers;
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
        bool isMobile = window.Width < 910 ? true : false;
        List<SliderItemModel> sliderItems = new List<SliderItemModel>();

        _sliderItemService.GetSlideList().Result.ForEach(async x =>
        {
            sliderItems.Add(new SliderItemModel
            {
                RouteLink = x.RouteLink,
                ImageAlt = x.ImageAlt,
                PictureUrl = isMobile ? 
                await _pictureService.GetPictureUrlAsync(x.MobilePictureId != null ? (int)x.MobilePictureId: (int)x.PictureId) :
                await _pictureService.GetPictureUrlAsync((int)x.PictureId),
                Order = x.Order
            });
        });

        return Json(JsonConvert.SerializeObject(sliderItems));
    }
}
