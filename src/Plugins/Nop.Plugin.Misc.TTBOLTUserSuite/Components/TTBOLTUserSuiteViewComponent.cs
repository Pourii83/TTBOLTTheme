using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Http;
using Nop.Plugin.Misc.TTBOLTUserSuite.Models;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.UI;

namespace Nop.Plugin.Misc.TTBOLTUserSuite.Components;

public class TTBOLTUserSuiteViewComponent : NopViewComponent
{
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly ILocalizationService _localizationService;
    private readonly MediaSettings _mediaSettings;
    private readonly INopHtmlHelper _nopHtmlHelper;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly IPictureService _pictureService;
    private readonly IWorkContext _workContext;

    public TTBOLTUserSuiteViewComponent(
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        MediaSettings mediaSettings,
        INopHtmlHelper nopHtmlHelper,
        INopUrlHelper nopUrlHelper,
        IPictureService pictureService,
        IWorkContext workContext)
    {
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _mediaSettings = mediaSettings;
        _nopHtmlHelper = nopHtmlHelper;
        _nopUrlHelper = nopUrlHelper;
        _pictureService = pictureService;
        _workContext = workContext;
    }

    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        if (widgetZone != PublicWidgetZones.CustomerInfoTop)
            return Content(string.Empty);

        var customer = await _workContext.GetCurrentCustomerAsync();
        var pictureId = await _genericAttributeService.GetAttributeAsync<int>(
            customer,
            NopCustomerDefaults.AvatarPictureIdAttribute);

        _nopHtmlHelper.AddCssFileParts("~/Plugins/Misc.TTBOLTUserSuite/Content/styles.css");

        var model = new ProfilePictureModel
        {
            PictureUrl = pictureId > 0
                ? await _pictureService.GetPictureUrlAsync(pictureId, _mediaSettings.AvatarPictureSize, false)
                : string.Empty,
            Title = await _localizationService.GetResourceAsync("Plugins.Misc.TTBOLTUserSuite.ProfilePicture.Title"),
            Description = await _localizationService.GetResourceAsync("Plugins.Misc.TTBOLTUserSuite.ProfilePicture.Description"),
            ActionText = await _localizationService.GetResourceAsync(pictureId > 0
                ? "Plugins.Misc.TTBOLTUserSuite.ProfilePicture.Change"
                : "Plugins.Misc.TTBOLTUserSuite.ProfilePicture.Add"),
            ActionUrl = _nopUrlHelper.RouteUrl(NopRouteNames.Standard.CUSTOMER_AVATAR)
        };

        return View("~/Plugins/Misc.TTBOLTUserSuite/Views/Components/ProfilePicture.cshtml", model);
    }
}
