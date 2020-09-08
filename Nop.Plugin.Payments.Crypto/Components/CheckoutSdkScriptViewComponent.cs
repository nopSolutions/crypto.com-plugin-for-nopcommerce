using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Plugin.Payments.Crypto.Services;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.Crypto.Components
{
    /// <summary>
    /// Represents a view component to generate the Crypto checkout sdk script
    /// </summary>
    [ViewComponent(Name = Defaults.CheckoutSdk.SCRIPT_VIEW_COMPONENT_NAME)]
    public class CheckoutSdkScriptViewComponent : NopViewComponent
    {
        #region Fields

        private readonly ICheckoutSdkScriptFactory _checkoutSdkScriptFactory;

        #endregion

        #region Ctor

        public CheckoutSdkScriptViewComponent(ICheckoutSdkScriptFactory checkoutSdkScriptFactory)
        {
            _checkoutSdkScriptFactory = checkoutSdkScriptFactory;
        }

        #endregion

        #region Methods

        public IViewComponentResult Invoke()
        {
            var script = _checkoutSdkScriptFactory.Create();
            return new HtmlContentViewComponentResult(script);
        }

        #endregion
    }
}
