using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.TTBOLTCarousel.Areas.Admin.Models;

public record SliderItemModel : BaseNopEntityModel
{
    [NopResourceDisplayName("Plugins.PW.HomeSlider.Fields.Language")]
    public int LanguageId { get; set; }
    [NopResourceDisplayName("Plugins.PW.HomeSlider.Fields.Language")]
    public string LanguageName { get; set; }

    [NopResourceDisplayName("Plugins.PW.HomeSlider.Fields.RouteLink")]
    public string RouteLink { get; set; }
    [NopResourceDisplayName("Plugins.PW.HomeSlider.Fields.Order")]
    public int Order { get; set; }
    [NopResourceDisplayName("Plugins.PW.HomeSlider.Fields.ImageAlt")]
    public string ImageAlt { get; set; }
    [NopResourceDisplayName("Plugins.PW.HomeSlider.Fields.Picture")]
    public IFormFile PictureFile { get; set; }
    [NopResourceDisplayName("Plugins.PW.HomeSlider.Fields.MobilePicture")]
    public IFormFile MobilePictureFile { get; set; }
    [NopResourceDisplayName("Plugins.PW.HomeSlider.Fields.PictureUrl")]
    public string PictureUrl { get; set; }
    [NopResourceDisplayName("Plugins.PW.HomeSlider.Fields.MobilePicture")]
    public string MobilePictureUrl { get; set; }

    public int? MobilePictureId { get; set; }
    public int? PictureId { get; set; }

    public IList<SelectListItem> AvailableLanguages { get; set; }
}
