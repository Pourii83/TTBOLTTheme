using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Widgets.TTBOLTCarousel.Areas.Admin.Models;

namespace Nop.Plugin.Widgets.TTBOLTCarousel.Areas.Admin.Factories;

/// <summary>
/// Represents the store pickup point models factory
/// </summary>
public interface ISliderItemModelFactory
{
    /// <summary>
    /// Prepare store pickup point list model
    /// </summary>
    /// <param name="searchModel">Store pickup point search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the store pickup point list model
    /// </returns>
    Task<SliderItemListModel> PrepareSliderItemListModelAsync(SliderItemSearchModel searchModel);

    /// <summary>
    /// Prepare store pickup point search model
    /// </summary>
    /// <param name="searchModel">Store pickup point search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the store pickup point search model
    /// </returns>
    Task<SliderItemSearchModel> PrepareSliderItemSearchModelAsync(SliderItemSearchModel searchModel);
    Task PrepareAvailableLanguagesAsync(IList<SelectListItem> items);

}