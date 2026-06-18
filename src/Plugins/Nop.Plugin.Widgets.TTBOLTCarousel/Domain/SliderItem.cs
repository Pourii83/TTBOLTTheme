using Nop.Core;

namespace Nop.Plugin.Widgets.TTBOLTCarousel.Domain;

public class SliderItem : BaseEntity
{
    public int LanguageId { get; set; }
    public string RouteLink { get; set; }
    public string ImageAlt { get; set; }
    public int? PictureId { get; set; }
    public int? MobilePictureId { get; set; }
    public int Order { get; set; }
}