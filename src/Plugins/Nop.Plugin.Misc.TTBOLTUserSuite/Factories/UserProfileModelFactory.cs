using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Plugin.Misc.TTBOLTUserSuite.Models;
using Nop.Services.Common;
using Nop.Services.Media;

namespace Nop.Plugin.Misc.TTBOLTUserSuite.Factories;

public class UserProfileModelFactory : IUserProfileModelFactory
{
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly MediaSettings _mediaSettings;
    private readonly IPictureService _pictureService;
    private readonly IWorkContext _workContext;

    public UserProfileModelFactory(
        IGenericAttributeService genericAttributeService,
        MediaSettings mediaSettings,
        IPictureService pictureService,
        IWorkContext workContext)
    {
        _genericAttributeService = genericAttributeService;
        _mediaSettings = mediaSettings;
        _pictureService = pictureService;
        _workContext = workContext;
    }

    public async Task<ProfilePictureModel> PrepareProfilePictureModelAsync()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var pictureId = await _genericAttributeService.GetAttributeAsync<int>(
            customer,
            NopCustomerDefaults.AvatarPictureIdAttribute);

        return new ProfilePictureModel
        {
            PictureUrl = pictureId > 0
                ? await _pictureService.GetPictureUrlAsync(pictureId, _mediaSettings.AvatarPictureSize, false)
                : string.Empty
        };
    }
}
