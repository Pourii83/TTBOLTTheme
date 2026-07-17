using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.TTBOLTUserSuite.Factories;

namespace Nop.Plugin.Misc.TTBOLTUserSuite.Infrastructure;

public class NopStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserProfileModelFactory, UserProfileModelFactory>();
    }

    public void Configure(IApplicationBuilder application)
    {
    }

    public int Order => 3000;
}
