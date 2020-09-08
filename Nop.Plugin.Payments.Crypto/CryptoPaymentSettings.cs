using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.Crypto
{
    /// <summary>
    /// Represents plugin settings
    /// </summary>
    public class CryptoPaymentSettings : ISettings
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether to use sandbox environment
        /// </summary>
        public bool UseSandbox { get; set; }

        /// <summary>
        /// Gets or sets a secret key
        /// </summary>
        public string SecretKey { get; set; }
        
        /// <summary>
        /// Gets or sets a publishable key
        /// </summary>
        public string PublishableKey { get; set; }

        /// <summary>
        /// Gets or sets a web hook signature secret key
        /// </summary>
        public string WebHookSignatureSecretKey { get; set; }

        /// <summary>
        /// Gets or sets a additional fee
        /// </summary>
        public decimal AdditionalFee { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        public bool AdditionalFeePercentage { get; set; }

        #endregion
    }
}
