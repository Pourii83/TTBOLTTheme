using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.TTBOLTUserSuite.Models;

public record ProfilePictureModel : BaseNopModel
{
    public string PictureUrl { get; set; }

    public bool HasPicture => !string.IsNullOrEmpty(PictureUrl);
}
