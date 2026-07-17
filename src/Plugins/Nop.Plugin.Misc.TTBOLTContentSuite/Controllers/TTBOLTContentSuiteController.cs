using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Misc.TTBOLTContentSuite.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class TTBOLTContentSuiteController : BasePluginController
{
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly ISettingService _settingService;
    private readonly TTBOLTContentSuiteSettings _settings;

    public TTBOLTContentSuiteController(
        ILocalizationService localizationService,
        INotificationService notificationService,
        ISettingService settingService,
        TTBOLTContentSuiteSettings settings)
    {
        _localizationService = localizationService;
        _notificationService = notificationService;
        _settingService = settingService;
        _settings = settings;
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public IActionResult Configure()
    {
        return View(
            "~/Plugins/Misc.TTBOLTContentSuite/Views/Configure.cshtml",
            PrepareConfigurationModel(
                _settings.TableOfContentsHeadingTags,
                _settings.NewsTableOfContentsHeadingTags));
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        _settings.TableOfContentsHeadingTags = NormalizeHeadingTags(model.SelectedHeadingTags);
        _settings.NewsTableOfContentsHeadingTags = NormalizeHeadingTags(model.SelectedNewsHeadingTags);
        await _settingService.SaveSettingAsync(_settings);

        _notificationService.SuccessNotification(
            await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return Configure();
    }

    private static ConfigurationModel PrepareConfigurationModel(
        IEnumerable<string> selectedHeadingTags,
        IEnumerable<string> selectedNewsHeadingTags)
    {
        var selectedTags = NormalizeHeadingTags(selectedHeadingTags);
        var selectedNewsTags = NormalizeHeadingTags(selectedNewsHeadingTags);
        var model = new ConfigurationModel
        {
            SelectedHeadingTags = selectedTags,
            SelectedNewsHeadingTags = selectedNewsTags
        };

        foreach (var headingTag in TTBOLTContentSuiteSettings.SupportedHeadingTags)
        {
            model.AvailableHeadingTags.Add(new SelectListItem
            {
                Text = headingTag.ToUpperInvariant(),
                Value = headingTag,
                Selected = selectedTags.Contains(headingTag)
            });

            model.AvailableNewsHeadingTags.Add(new SelectListItem
            {
                Text = headingTag.ToUpperInvariant(),
                Value = headingTag,
                Selected = selectedNewsTags.Contains(headingTag)
            });
        }

        return model;
    }

    private static List<string> NormalizeHeadingTags(IEnumerable<string> headingTags)
    {
        var selectedTags = (headingTags ?? Enumerable.Empty<string>())
            .Select(headingTag => headingTag?.Trim().ToLowerInvariant())
            .Where(headingTag => TTBOLTContentSuiteSettings.SupportedHeadingTags.Contains(headingTag))
            .Distinct()
            .ToHashSet();

        return TTBOLTContentSuiteSettings.SupportedHeadingTags
            .Where(selectedTags.Contains)
            .ToList();
    }
}
