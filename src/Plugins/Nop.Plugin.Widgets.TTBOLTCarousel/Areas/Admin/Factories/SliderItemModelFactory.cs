using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Widgets.TTBOLTCarousel.Areas.Admin.Models;
using Nop.Plugin.Widgets.TTBOLTCarousel.Areas.Admin.Services;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Widgets.TTBOLTCarousel.Areas.Admin.Factories;

public class SliderItemModelFactory : ISliderItemModelFactory
{
    private readonly ILanguageService _languageService;
    private readonly IPictureService _pictureService;
    private readonly ISliderItemService _sliderItemService;

    public SliderItemModelFactory(ISliderItemService sliderItemService,
        ILanguageService languageService,
        IPictureService pictureService)
    {
        _sliderItemService = sliderItemService;
        _languageService = languageService;
        _pictureService = pictureService;
    }

    public async Task<SliderItemListModel> PrepareSliderItemListModelAsync(SliderItemSearchModel searchModel)
    {
        var slides = await _sliderItemService.GetAllSlides(pageIndex: searchModel.Page - 1,
            pageSize: searchModel.PageSize);
        var languages = await _languageService.GetAllLanguagesAsync();

        var model = await new SliderItemListModel().PrepareToGridAsync(searchModel, slides, () =>
        {
            return slides.SelectAwait(async slide =>
            {
                return new SliderItemModel
                {
                    Id = slide.Id,
                    LanguageName = languages.FirstOrDefault(lang => lang.Id == slide.LanguageId)?.Name,
                    Order = slide.Order,
                    PictureId = slide.PictureId ?? 0,
                    MobilePictureId = slide.MobilePictureId ?? 0,
                    ImageAlt = slide.ImageAlt,
                    RouteLink = slide.RouteLink,
                    PictureUrl = slide.PictureId.HasValue
                        ? await _pictureService.GetPictureUrlAsync(slide.PictureId.Value)
                        : await _pictureService.GetDefaultPictureUrlAsync(),
                    MobilePictureUrl = slide.PictureId.HasValue
                        ? await _pictureService.GetPictureUrlAsync(slide.MobilePictureId ?? slide.PictureId.Value)
                        : await _pictureService.GetDefaultPictureUrlAsync()
                };
            });
        });

        return model;
    }

    public Task<SliderItemSearchModel> PrepareSliderItemSearchModelAsync(SliderItemSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        searchModel.SetGridPageSize();

        return Task.FromResult(searchModel);
    }

    public async Task PrepareAvailableLanguagesAsync(IList<SelectListItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        var languages = await _languageService.GetAllLanguagesAsync();
        foreach (var language in languages)
        {
            items.Add(new SelectListItem
            {
                Text = language.Name,
                Value = language.Id.ToString()
            });
        }
    }
}
