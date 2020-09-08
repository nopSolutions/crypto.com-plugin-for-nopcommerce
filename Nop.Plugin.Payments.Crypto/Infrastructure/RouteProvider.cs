using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.Crypto.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(
                Defaults.ConfigurationRouteName, 
                "Plugins/Crypto/Configure",
                new 
                { 
                    controller = "CryptoConfiguration", 
                    action = "Configure", 
                    area = AreaNames.Admin 
                });

            endpointRouteBuilder.MapControllerRoute(
                Defaults.WebHooks.RouteName,
                "Plugins/Crypto/Webhook",
                new
                {
                    controller = "CryptoWebHook",
                    action = "Handle"
                });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}