using Nop.Core.Events;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Customer;

namespace Nop.Plugin.Misc.TTBOLTUserSuite.Events;

public class CustomerNavigationEventConsumer : IConsumer<ModelPreparedEvent<BaseNopModel>>
{
    private readonly ILocalizationService _localizationService;
    private readonly TTBOLTUserSuiteSettings _settings;

    public CustomerNavigationEventConsumer(
        ILocalizationService localizationService,
        TTBOLTUserSuiteSettings settings)
    {
        _localizationService = localizationService;
        _settings = settings;
    }

    public async Task HandleEventAsync(ModelPreparedEvent<BaseNopModel> eventMessage)
    {
        if (!_settings.Enabled || eventMessage.Model is not CustomerNavigationModel model)
            return;

        if (model.CustomerNavigationItems.Any(item => item.Tab == TTBOLTUserSuiteDefaults.ProfilePictureMenuTab))
            return;

        var infoItem = model.CustomerNavigationItems.FirstOrDefault(item => item.Tab == (int)CustomerNavigationEnum.Info);
        var position = infoItem == null ? 0 : model.CustomerNavigationItems.IndexOf(infoItem) + 1;

        model.CustomerNavigationItems.Insert(position, new CustomerNavigationItemModel
        {
            RouteName = TTBOLTUserSuiteDefaults.ProfilePictureRouteName,
            Title = await _localizationService.GetResourceAsync("Plugins.Misc.TTBOLTUserSuite.ProfilePicture"),
            Tab = TTBOLTUserSuiteDefaults.ProfilePictureMenuTab,
            ItemClass = TTBOLTUserSuiteDefaults.ProfilePictureMenuClass
        });
    }
}
