using Nop.Core.Domain.Cms;
using Nop.Core.Domain.News;
using Nop.Plugin.Misc.TTBOLTContentSuite.Components;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Misc.TTBOLTContentSuite;

public class TTBOLTContentSuitePlugin : BasePlugin, IWidgetPlugin
{
    private readonly ILocalizationService _localizationService;
    private readonly NewsSettings _newsSettings;
    private readonly ISettingService _settingService;
    private readonly WidgetSettings _widgetSettings;

    public TTBOLTContentSuitePlugin(
        ILocalizationService localizationService,
        NewsSettings newsSettings,
        ISettingService settingService,
        WidgetSettings widgetSettings)
    {
        _localizationService = localizationService;
        _newsSettings = newsSettings;
        _settingService = settingService;
        _widgetSettings = widgetSettings;
    }

    public bool HideInWidgetList => false;

    public Type GetWidgetViewComponent(string widgetZone)
    {
        return typeof(TTBOLTContentSuiteViewComponent);
    }

    public Task<IList<string>> GetWidgetZonesAsync()
    {
        return Task.FromResult<IList<string>>(new List<string>
        {
            PublicWidgetZones.HomepageBeforeNews,
            PublicWidgetZones.BlogListPageBeforePost,
            PublicWidgetZones.LeftSideColumnBlogBefore,
            PublicWidgetZones.BlogPostPageBeforeBody,
            PublicWidgetZones.NewsItemPageBeforeBody,
            PublicWidgetZones.NewsListPageInsideItem,
            AdminWidgetZones.BlogPostDetailsBlock,
            AdminWidgetZones.NewsItemsDetailsBlock
        });
    }

    public override async Task InstallAsync()
    {
        _newsSettings.MainPageNewsCount = 4;
        await _settingService.SaveSettingAsync(_newsSettings, settings => settings.MainPageNewsCount);

        if (!_widgetSettings.ActiveWidgetSystemNames.Contains(PluginDescriptor.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(PluginDescriptor.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        await _localizationService.AddOrUpdateLocaleResourceAsync(GetLocaleResources());

        await base.InstallAsync();
    }

    public override async Task UpdateAsync(string currentVersion, string targetVersion)
    {
        await _localizationService.AddOrUpdateLocaleResourceAsync(GetLocaleResources());
        await base.UpdateAsync(currentVersion, targetVersion);
    }

    public override async Task UninstallAsync()
    {
        if (_widgetSettings.ActiveWidgetSystemNames.Contains(PluginDescriptor.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Remove(PluginDescriptor.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.TTBOLTContentSuite");

        await base.UninstallAsync();
    }

    private static Dictionary<string, string> GetLocaleResources()
    {
        return new Dictionary<string, string>
        {
            ["Plugins.Misc.TTBOLTContentSuite.BlogPost.Thumbnail"] = "تصاویر مقاله",
            ["Plugins.Misc.TTBOLTContentSuite.BlogPost.Picture"] = "تصویر اصلی",
            ["Plugins.Misc.TTBOLTContentSuite.BlogPost.ThumbnailPicture"] = "تصویر بندانگشتی",
            ["Plugins.Misc.TTBOLTContentSuite.BlogPost.RelatedPosts"] = "مقالات مرتبط",
            ["Plugins.Misc.TTBOLTContentSuite.BlogPost.RelatedPosts.Hint"] = "مقاله‌های مرتبط را از فهرست انتخاب کنید. چهار مورد نخست در صفحه مقاله نمایش داده می‌شوند.",
            ["Plugins.Misc.TTBOLTContentSuite.BlogPost.RandomPosts"] = "مقالات تصادفی",
            ["Plugins.Misc.TTBOLTContentSuite.NewsItem.Thumbnail"] = "تصاویر خبر",
            ["Plugins.Misc.TTBOLTContentSuite.NewsItem.Picture"] = "تصویر اصلی",
            ["Plugins.Misc.TTBOLTContentSuite.NewsItem.ThumbnailPicture"] = "تصویر بندانگشتی",
            ["Plugins.Misc.TTBOLTContentSuite.HomePageBlogs.Title"] = "مقالات جدید",
            ["Plugins.Misc.TTBOLTContentSuite.HomePageBlogs.ReadMore"] = "بیشتر بدانید"
        };
    }
}
