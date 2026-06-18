using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Media;
using Nop.Plugin.Widgets.TTBOLTCarousel.Areas.Admin.Factories;
using Nop.Plugin.Widgets.TTBOLTCarousel.Areas.Admin.Models;
using Nop.Plugin.Widgets.TTBOLTCarousel.Areas.Admin.Services;
using Nop.Plugin.Widgets.TTBOLTCarousel.Domain;
using Nop.Services.Media;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.TTBOLTCarousel.Areas.Admin.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class TTBOLTCarouselController : BasePluginController
{
    private readonly ISliderItemModelFactory _sliderItemModelFactory;
    private readonly IPictureService _pictureService;
    private readonly ISliderItemService _sliderItemService;

    public TTBOLTCarouselController(ISliderItemModelFactory sliderItemModelFactory,
        IPictureService pictureService,
        ISliderItemService sliderItemService)
    {
        _sliderItemModelFactory = sliderItemModelFactory;
        _pictureService = pictureService;
        _sliderItemService = sliderItemService;
    }
    public async Task<IActionResult> Configure()
    {
        var model = await _sliderItemModelFactory.PrepareSliderItemSearchModelAsync(new SliderItemSearchModel());

        return View("~/Plugins/Widgets.TTBOLTCarousel/Areas/Admin/Views/Configure.cshtml", model);
    }
    [HttpPost]
    public async Task<IActionResult> List(SliderItemSearchModel searchModel)
    {
        //prepare model
        var model = await _sliderItemModelFactory.PrepareSliderItemListModelAsync(searchModel);

        return Json(model);
    }
    public async Task<IActionResult> Create()
    {
        var model = new SliderItemModel();

        model.AvailableLanguages = new List<SelectListItem>();
        await _sliderItemModelFactory.PrepareAvailableLanguagesAsync(model.AvailableLanguages);

        return View("~/Plugins/Widgets.TTBOLTCarousel/Areas/Admin/Views/Create.cshtml", model);

    }
    [HttpPost]
    public async Task<IActionResult> Create(SliderItemModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableLanguages = new List<SelectListItem>();
            await _sliderItemModelFactory.PrepareAvailableLanguagesAsync(model.AvailableLanguages);

            return View("~/Plugins/Widgets.TTBOLTCarousel/Areas/Admin/Views/Create.cshtml", model);
        }

        Picture picture = null;
        Picture mobilePicture = null;

        if (model.PictureFile != null)
          picture = await _pictureService.InsertPictureAsync(model.PictureFile);
        else
            model.PictureId = null;

        if (model.MobilePictureFile != null)
            mobilePicture = await _pictureService.InsertPictureAsync(model.MobilePictureFile);
        else
            model.MobilePictureId = null;

        var sliderItem = new SliderItem
        {
            LanguageId = model.LanguageId,
            Order = model.Order,
            PictureId = model.PictureFile != null ? picture.Id : null,
            MobilePictureId = model.MobilePictureFile != null ? mobilePicture.Id : null,
            ImageAlt = model.ImageAlt,
            RouteLink = model.RouteLink,
        };

        await _sliderItemService.InsertSlideAsync(sliderItem);

        model.AvailableLanguages = new List<SelectListItem>();
        await _sliderItemModelFactory.PrepareAvailableLanguagesAsync(model.AvailableLanguages);

        ViewBag.RefreshPage = true;
        return View("~/Plugins/Widgets.TTBOLTCarousel/Areas/Admin/Views/Create.cshtml", model);

    }

    public async Task<IActionResult> Edit(int id)
    {
        var sliderItem = await _sliderItemService.GetSlideByIdAsync(id);
        if (sliderItem == null)
            return RedirectToAction("Configure");

        var model = new SliderItemModel
        {
            Id = sliderItem.Id,
            LanguageId = sliderItem.LanguageId,
            Order = sliderItem.Order,
            PictureId = sliderItem.PictureId,
            MobilePictureId = sliderItem.MobilePictureId,
            ImageAlt = sliderItem.ImageAlt,
            RouteLink = sliderItem.RouteLink,
        };

        model.AvailableLanguages = new List<SelectListItem>();
        await _sliderItemModelFactory.PrepareAvailableLanguagesAsync(model.AvailableLanguages);
        return View("~/Plugins/Widgets.TTBOLTCarousel/Areas/Admin/Views/Edit.cshtml", model);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(SliderItemModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableLanguages = new List<SelectListItem>();
            await _sliderItemModelFactory.PrepareAvailableLanguagesAsync(model.AvailableLanguages);

            return View("~/Plugins/Widgets.TTBOLTCarousel/Areas/Admin/Views/Edit.cshtml", model);
        }

        Picture picture = null;
        Picture mobilePicture = null;

        if (model.PictureFile != null)
            picture = await _pictureService.InsertPictureAsync(model.PictureFile);
        if (model.MobilePictureFile != null)
            mobilePicture = await _pictureService.InsertPictureAsync(model.MobilePictureFile);

        var sliderItem = new SliderItem
        {
            Id = model.Id,
            LanguageId = model.LanguageId,
            Order = model.Order,
            PictureId = model.PictureFile != null ? picture.Id : model.PictureId,
            MobilePictureId = model.MobilePictureFile != null ? mobilePicture.Id :
                                model.MobilePictureId ?? model.PictureId,
            ImageAlt = model.ImageAlt,
            RouteLink = model.RouteLink
        };

        await _sliderItemService.UpdateSlideAsync(sliderItem);
        ViewBag.RefreshPage = true;
        model.AvailableLanguages = new List<SelectListItem>();
        await _sliderItemModelFactory.PrepareAvailableLanguagesAsync(model.AvailableLanguages);
        return View("~/Plugins/Widgets.TTBOLTCarousel/Areas/Admin/Views/Edit.cshtml", model);
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var sliderItem = await _sliderItemService.GetSlideByIdAsync(id);
        if (sliderItem == null)
            return RedirectToAction("Configure");

        if (sliderItem.PictureId != null)
        {
            var picture = await _pictureService.GetPictureByIdAsync((int)sliderItem.PictureId);
            if (picture != null)
                await _pictureService.DeletePictureAsync(picture);
        }

        if (sliderItem.MobilePictureId != null && sliderItem.MobilePictureId != sliderItem.PictureId)
        {
            var mobilePicture = await _pictureService.GetPictureByIdAsync((int)sliderItem.MobilePictureId);
            if (mobilePicture != null)
                await _pictureService.DeletePictureAsync(mobilePicture);
        }

        await _sliderItemService.DeleteSlideAsync(sliderItem);

        return new NullJsonResult();
    }
}
