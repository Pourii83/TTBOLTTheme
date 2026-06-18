using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.TTBOLTCarousel.Areas.Admin.Models;

public record SliderItemModel : BaseNopEntityModel
{
    [NopResourceDisplayName("Plugins.Widgets.TTBOLTCarousel.Fields.Language")]
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.TTBOLTCarousel.Fields.Language")]
    public string LanguageName { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.TTBOLTCarousel.Fields.RouteLink")]
    public string RouteLink { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.TTBOLTCarousel.Fields.Order")]
    public int Order { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.TTBOLTCarousel.Fields.ImageAlt")]
    public string ImageAlt { get; set; }

    [UIHint("Picture")]
    [NopResourceDisplayName("Plugins.Widgets.TTBOLTCarousel.Fields.Picture")]
    public int PictureId { get; set; }

    [UIHint("Picture")]
    [NopResourceDisplayName("Plugins.Widgets.TTBOLTCarousel.Fields.MobilePicture")]
    public int MobilePictureId { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.TTBOLTCarousel.Fields.PictureUrl")]
    public string PictureUrl { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.TTBOLTCarousel.Fields.MobilePicture")]
    public string MobilePictureUrl { get; set; }

    public IList<SelectListItem> AvailableLanguages { get; set; }
}
