using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.TTBOLTContentSuite.Factories;
using Nop.Plugin.Misc.TTBOLTContentSuite.Infrastructure;
using Nop.Plugin.Misc.TTBOLTContentSuite.Services;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Infrastructure;

public class NopStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IContentSuiteModelFactory, ContentSuiteModelFactory>();
        services.AddScoped<IRelatedBlogPostService, RelatedBlogPostService>();

        services.Configure<RazorViewEngineOptions>(options =>
        {
            options.ViewLocationExpanders.Add(new ContentSuiteViewLocationExpander());
        });
    }

    public void Configure(IApplicationBuilder application)
    {
    }

    public int Order => 3000;
}
