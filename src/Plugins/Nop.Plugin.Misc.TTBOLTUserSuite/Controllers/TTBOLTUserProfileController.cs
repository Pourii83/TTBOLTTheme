using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.TTBOLTUserSuite.Factories;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Web.Controllers;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Misc.TTBOLTUserSuite.Controllers;

[AutoValidateAntiforgeryToken]
public class TTBOLTUserProfileController : BasePublicController
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp"
    };

    private readonly ICustomerService _customerService;
    private readonly IDownloadService _downloadService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly IPictureService _pictureService;
    private readonly IUserProfileModelFactory _userProfileModelFactory;
    private readonly IWorkContext _workContext;
    private readonly TTBOLTUserSuiteSettings _settings;

    public TTBOLTUserProfileController(
        ICustomerService customerService,
        IDownloadService downloadService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPictureService pictureService,
        IUserProfileModelFactory userProfileModelFactory,
        IWorkContext workContext,
        TTBOLTUserSuiteSettings settings)
    {
        _customerService = customerService;
        _downloadService = downloadService;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _pictureService = pictureService;
        _userProfileModelFactory = userProfileModelFactory;
        _workContext = workContext;
        _settings = settings;
    }

    [HttpGet]
    public async Task<IActionResult> ProfilePicture()
    {
        var accessResult = await CheckAccessAsync();
        if (accessResult != null)
            return accessResult;

        return View("~/Plugins/Misc.TTBOLTUserSuite/Views/ProfilePicture.cshtml",
            await _userProfileModelFactory.PrepareProfilePictureModelAsync());
    }

    [HttpPost]
    [FormValueRequired("upload-profile-picture")]
    public async Task<IActionResult> ProfilePicture(IFormFile uploadedFile)
    {
        var accessResult = await CheckAccessAsync();
        if (accessResult != null)
            return accessResult;

        if (uploadedFile == null || uploadedFile.Length == 0)
            ModelState.AddModelError(string.Empty,
                await _localizationService.GetResourceAsync("Plugins.Misc.TTBOLTUserSuite.ProfilePicture.Required"));
        else if (!AllowedContentTypes.Contains(uploadedFile.ContentType))
            ModelState.AddModelError(string.Empty,
                await _localizationService.GetResourceAsync("Plugins.Misc.TTBOLTUserSuite.ProfilePicture.InvalidFormat"));
        else if (uploadedFile.Length > _settings.MaximumPictureSizeBytes)
            ModelState.AddModelError(string.Empty,
                await _localizationService.GetResourceAsync("Plugins.Misc.TTBOLTUserSuite.ProfilePicture.FileTooLarge"));

        if (!ModelState.IsValid)
            return View("~/Plugins/Misc.TTBOLTUserSuite/Views/ProfilePicture.cshtml",
                await _userProfileModelFactory.PrepareProfilePictureModelAsync());

        var customer = await _workContext.GetCurrentCustomerAsync();
        var currentPictureId = await _genericAttributeService.GetAttributeAsync<int>(
            customer,
            NopCustomerDefaults.AvatarPictureIdAttribute);
        var currentPicture = await _pictureService.GetPictureByIdAsync(currentPictureId);
        var pictureBinary = await _downloadService.GetDownloadBitsAsync(uploadedFile);

        var picture = currentPicture == null
            ? await _pictureService.InsertPictureAsync(pictureBinary, uploadedFile.ContentType, uploadedFile.FileName)
            : await _pictureService.UpdatePictureAsync(currentPicture.Id, pictureBinary, uploadedFile.ContentType, uploadedFile.FileName);

        await _genericAttributeService.SaveAttributeAsync(
            customer,
            NopCustomerDefaults.AvatarPictureIdAttribute,
            picture.Id);

        _notificationService.SuccessNotification(
            await _localizationService.GetResourceAsync("Plugins.Misc.TTBOLTUserSuite.ProfilePicture.Saved"));

        return RedirectToRoute(TTBOLTUserSuiteDefaults.ProfilePictureRouteName);
    }

    [HttpPost, ActionName("ProfilePicture")]
    [FormValueRequired("remove-profile-picture")]
    public async Task<IActionResult> RemoveProfilePicture()
    {
        var accessResult = await CheckAccessAsync();
        if (accessResult != null)
            return accessResult;

        var customer = await _workContext.GetCurrentCustomerAsync();
        var pictureId = await _genericAttributeService.GetAttributeAsync<int>(
            customer,
            NopCustomerDefaults.AvatarPictureIdAttribute);
        var picture = await _pictureService.GetPictureByIdAsync(pictureId);

        if (picture != null)
            await _pictureService.DeletePictureAsync(picture);

        await _genericAttributeService.SaveAttributeAsync(
            customer,
            NopCustomerDefaults.AvatarPictureIdAttribute,
            0);

        _notificationService.SuccessNotification(
            await _localizationService.GetResourceAsync("Plugins.Misc.TTBOLTUserSuite.ProfilePicture.Removed"));

        return RedirectToRoute(TTBOLTUserSuiteDefaults.ProfilePictureRouteName);
    }

    private async Task<IActionResult> CheckAccessAsync()
    {
        if (!_settings.Enabled)
            return NotFound();

        var customer = await _workContext.GetCurrentCustomerAsync();
        return await _customerService.IsRegisteredAsync(customer) ? null : Challenge();
    }
}
