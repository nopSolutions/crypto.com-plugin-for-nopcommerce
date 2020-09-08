using Nop.Core;

namespace Nop.Plugin.Payments.Crypto
{
    /// <summary>
    /// Represents plugin defaults
    /// </summary>
    public static class Defaults
    {
        /// <summary>
        /// Gets the plugin system name
        /// </summary>
        public static string SystemName => "Payments.Crypto";

        /// <summary>
        /// Gets the plugin configuration route name
        /// </summary>
        public static string ConfigurationRouteName => "Plugin.Payments.Crypto.Configure";

        /// <summary>
        /// Gets a name of the view component to display payment info in public store
        /// </summary>
        public const string PAYMENT_INFO_VIEW_COMPONENT_NAME = "CryptoPaymentInfo";

        /// <summary>
        /// Gets the session key to get process payment request
        /// </summary>
        public static string PaymentRequestSessionKey => "OrderPaymentInfo";

        /// <summary>
        /// Gets the name of a generic attribute to store the refund identifier
        /// </summary>
        public static string RefundIdAttributeName => "CryptoRefundId";

        /// <summary>
        /// Represents API defaults
        /// </summary>
        public static class Api
        {
            /// <summary>
            /// Gets the API base URL
            /// </summary>
            public static string BaseUrl => "https://pay.crypto.com";

            /// <summary>
            /// Gets the user agent
            /// </summary>
            public static string UserAgent => $"nopCommerce-{NopVersion.CurrentVersion}";

            /// <summary>
            /// Gets the default timeout
            /// </summary>
            public static int DefaultTimeout => 20;

            /// <summary>
            /// Represents endpoints defaults
            /// </summary>
            public static class Endpoints
            {
                /// <summary>
                /// Gets the API payment endpoint path
                /// </summary>
                public static string PaymentPath => "/api/payments";

                /// <summary>
                /// Gets the API refund endpoint path
                /// </summary>
                public static string RefundPath => "/api/refunds";
            }
        }

        /// <summary>
        /// Represents a Crypto checkout SDK defaults
        /// </summary>
        public static class CheckoutSdk
        {
            /// <summary>
            /// Gets the Crypto.com Pay Checkout SDK script URL
            /// </summary>
            public static string ScriptUrl => "https://js.crypto.com/sdk";

            /// <summary>
            /// Gets a name of the view component to add script to pages
            /// </summary>
            public const string SCRIPT_VIEW_COMPONENT_NAME = "CryptoCheckoutSdkScript";
        }

        /// <summary>
        /// Represents a web hooks defaults
        /// </summary>
        public static class WebHooks
        {
            /// <summary>
            /// Gets the signature header name
            /// </summary>
            public static string SignatureHeaderName => "Pay-Signature";

            /// <summary>
            /// Gets the route name
            /// </summary>
            public static string RouteName => "Plugin.Payments.Crypto.WebHook.Handle";
        }
    }
}
