using Nop.Plugin.Misc.TTBOLTUserSuite.Models;

namespace Nop.Plugin.Misc.TTBOLTUserSuite.Factories;

public interface IUserProfileModelFactory
{
    Task<ProfilePictureModel> PrepareProfilePictureModelAsync();
}
