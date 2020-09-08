using Microsoft.AspNetCore.Html;

namespace Nop.Plugin.Payments.Crypto.Services
{
    /// <summary>
    /// Provides an default implementation for factory to create the checkout sdk script
    /// </summary>
    public class DefaultCheckoutSdkScriptFactory : ICheckoutSdkScriptFactory
    {
        #region Fields

        private readonly CryptoPaymentSettings _cryptoPaymentSettings;

        #endregion

        #region Ctor

        public DefaultCheckoutSdkScriptFactory(CryptoPaymentSettings cryptoPaymentSettings)
        {
            _cryptoPaymentSettings = cryptoPaymentSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the checkout sdk script
        /// </summary>
        /// <returns>The checkout sdk script</returns>
        public virtual IHtmlContent Create()
        {
            if (string.IsNullOrWhiteSpace(_cryptoPaymentSettings.PublishableKey))
                return new HtmlString(string.Empty);

            return new HtmlString($"<script src=\"{Defaults.CheckoutSdk.ScriptUrl}?publishable-key={_cryptoPaymentSettings.PublishableKey}\"></script>");
        }

        #endregion
    }
}
