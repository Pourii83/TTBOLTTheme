using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.TTBOLTUserSuite.Models;

public record ProfilePictureModel : BaseNopModel
{
    public string PictureUrl { get; init; }

    public bool HasPicture => !string.IsNullOrEmpty(PictureUrl);

    public string Title { get; init; }

    public string Description { get; init; }

    public string ActionText { get; init; }

    public string ActionUrl { get; init; }
}
