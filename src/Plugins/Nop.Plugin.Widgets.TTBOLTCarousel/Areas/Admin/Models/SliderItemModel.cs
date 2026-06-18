using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.TTBOLTCarousel.Areas.Admin.Models;

public record SliderItemModel : BaseNopEntityModel
{
    [NopResourceDisplayName("Plugins.Widgets.TTBOLTCarousel.Fields.Language")]
    public int LanguageId { get; set; }
    [NopResourceDisplayName("Plugins.Widgets.TTBOLTCarousel.Language")]
    public string LanguageName { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.TTBOLTCarousel.RouteLink")]
    public string RouteLink { get; set; }
    [NopResourceDisplayName("Plugins.Widgets.TTBOLTCarousel.Order")]
    public int Order { get; set; }
    [NopResourceDisplayName("Plugins.Widgets.TTBOLTCarousel.ImageAlt")]
    public string ImageAlt { get; set; }
    [NopResourceDisplayName("Plugins.Widgets.TTBOLTCarousel.Picture")]
    public IFormFile PictureFile { get; set; }
    [NopResourceDisplayName("Plugins.Widgets.TTBOLTCarousel.MobilePicture")]
    public IFormFile MobilePictureFile { get; set; }
    [NopResourceDisplayName("Plugins.Widgets.TTBOLTCarousel.PictureUrl")]
    public string PictureUrl { get; set; }
    [NopResourceDisplayName("Plugins.Widgets.TTBOLTCarousel.MobilePicture")]
    public string MobilePictureUrl { get; set; }

    public int? MobilePictureId { get; set; }
    public int? PictureId { get; set; }

    public IList<SelectListItem> AvailableLanguages { get; set; }
}
