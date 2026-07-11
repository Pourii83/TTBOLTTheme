using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace Nop.Plugin.Misc.TTBOLTUserSuite.Infrastructure;

public class RouteProvider : BaseRouteProvider, IRouteProvider
{
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        var languagePattern = GetLanguageRoutePattern();

        endpointRouteBuilder.MapControllerRoute(
            TTBOLTUserSuiteDefaults.ProfilePictureRouteName,
            $"{languagePattern}/customer/profile-picture",
            new { controller = "TTBOLTUserProfile", action = "ProfilePicture" });
    }

    public int Priority => 0;
}
