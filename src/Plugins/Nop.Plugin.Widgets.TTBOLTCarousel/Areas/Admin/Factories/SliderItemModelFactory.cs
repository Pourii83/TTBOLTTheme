using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.TTBOLTCarousel.Areas.Admin.Models;
using Nop.Plugin.Widgets.TTBOLTCarousel.Areas.Admin.Services;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Stores;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Widgets.TTBOLTCarousel.Areas.Admin.Factories;

/// <summary>
/// Represents store pickup point models factory implementation
/// </summary>
public class SliderItemModelFactory : ISliderItemModelFactory
{
    private readonly ISliderItemService _sliderItemService;
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly IStoreService _storeService;
    private readonly IPictureService _pictureService;
    private readonly ILanguageService _languageService;


    #endregion

    #region Ctor

    public SliderItemModelFactory(ISliderItemService sliderItemService,
        ILocalizationService localizationService,
        ILanguageService languageService,
        IPictureService pictureService,
        IStoreService storeService)
    {
        _sliderItemService = sliderItemService;
        _localizationService = localizationService;
        _storeService = storeService;
        _pictureService = pictureService;
        _languageService = languageService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare store pickup point list model
    /// </summary>
    /// <param name="searchModel">Store pickup point search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the store pickup point list model
    /// </returns>
    public async Task<SliderItemListModel> PrepareSliderItemListModelAsync(SliderItemSearchModel searchModel)
    {
        var slides = await _sliderItemService.GetAllSlides(pageIndex: searchModel.Page - 1,
            pageSize: searchModel.PageSize);
        var language = EngineContext.Current.Resolve<IWorkContext>().GetWorkingLanguageAsync().Result;
        var model = await new SliderItemListModel().PrepareToGridAsync(searchModel, slides, () =>
        {
            return slides.SelectAwait(async slide =>
            {
                var store = await _storeService.GetStoreByIdAsync(slide.Id);

                return new SliderItemModel
                {

                    Id = slide.Id,
                    LanguageName = _languageService.GetAllLanguages().FirstOrDefault(lang => lang.Id == slide.LanguageId).Name,
                    Order = slide.Order,
                    PictureId = slide.PictureId,
                    MobilePictureId = slide.MobilePictureId,
                    ImageAlt = slide.ImageAlt,
                    RouteLink = slide.RouteLink,
                    PictureUrl = slide.PictureId != null ?
                    await _pictureService.GetPictureUrlAsync((int)slide.PictureId) : await _pictureService.GetDefaultPictureUrlAsync(),
                    MobilePictureUrl = slide.PictureId != null ?
                    await _pictureService.GetPictureUrlAsync(slide.MobilePictureId != null ?
                                                            (int)slide.MobilePictureId : (int)slide.PictureId) :
                    await _pictureService.GetDefaultPictureUrlAsync()
                };
            });
        });

        return model;
    }
    /// <summary>
    /// Prepare store pickup point search model
    /// </summary>
    /// <param name="searchModel">Store pickup point search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the store pickup point search model
    /// </returns>
    public Task<SliderItemSearchModel> PrepareSliderItemSearchModelAsync(SliderItemSearchModel searchModel)
    {
        if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

        //prepare page parameters
        searchModel.SetGridPageSize();

        return Task.FromResult(searchModel);
    }
    public async Task PrepareAvailableLanguagesAsync(IList<SelectListItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        //prepare available languages
        var languages = _languageService.GetAllLanguagesAsync().Result;
        foreach (var language in languages)
        {
            items.Add(new SelectListItem
            {
                Text = language.Name,
                Value = language.Id.ToString()
            });
        }
    }
    #endregion
}
