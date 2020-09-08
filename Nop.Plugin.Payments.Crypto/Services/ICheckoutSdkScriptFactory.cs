using Microsoft.AspNetCore.Html;

namespace Nop.Plugin.Payments.Crypto.Services
{
    /// <summary>
    /// Provides an abstraction for factory to create the checkout sdk script
    /// </summary>
    public interface ICheckoutSdkScriptFactory
    {
        /// <summary>
        /// Creates the checkout sdk script
        /// </summary>
        /// <returns>The checkout sdk script</returns>
        IHtmlContent Create();
    }
}
